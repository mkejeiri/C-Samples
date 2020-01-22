﻿using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MeterReaderLib;
using MeterReaderLib.Models;
using MeterReaderWeb.Data;
using MeterReaderWeb.Data.Entities;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace MeterReaderWeb.Services
{
    [Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)] 
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MeterService : Services.MeterReadingService.MeterReadingServiceBase
    {
        private readonly ILogger<MeterService> _logger;
        private readonly IReadingRepository _repository;
        private readonly JwtTokenValidationService _jwtTokenValidationService;

        public MeterService(ILogger<MeterService> logger, IReadingRepository repository, JwtTokenValidationService jwtTokenValidationService)
        {
            _logger = logger;
            _repository = repository;
            _jwtTokenValidationService = jwtTokenValidationService;
        }

        [AllowAnonymous]
        public override async  Task<TokenResponse> CreateToken(TokenRequest request, 
                                            ServerCallContext context)
        {
            var creds = new CredentialModel() {UserName = request.Username, Passcode = request.Password};
            var response = await _jwtTokenValidationService.GenerateTokenModelAsync(creds);

            if (response.Success)
            {
                return new TokenResponse()
                {
                    Token = response.Token,
                    Expiration = Timestamp.FromDateTime(response.Expiration),
                    Success = true
                };
            }
            return new TokenResponse()
            {
                Success = false
            };
        } 

        public override async Task<Empty> SendDiagnostics(IAsyncStreamReader<ReadingMessage> requestStream, 
                            ServerCallContext context)
        {
            var t = Task.Run(async () =>
            {
                await foreach (var reading in requestStream.ReadAllAsync())
                {
                    _logger.LogInformation($"Received Reading: {reading}");
                }
            });

            await t;
            return  new Empty();
        }

        public override async Task<StatusMessage> AddReading(ReadingPacket request, ServerCallContext context)
        {
             
            var result = new StatusMessage()
            {
                Success = ReadingStatus.Failure
            };
            if (request.Successful == ReadingStatus.Success)
            {
                try
                {

                    foreach (var r in request.Readings)
                    {
                        if (r.ReadingValue < 1000)
                        {
                            _logger.LogDebug("Reading value below acceptable level");
                            //throw new RpcException(Status.DefaultCancelled, "Value too low");
                            var trailer= new Metadata()
                            {
                                {"BadValue" , r.ReadingValue.ToString()},   
                                {"Field" , "ReadingValue"},   
                                {"Message" , "Readings are invalid"}   
                            };
                            throw new RpcException(new Status(StatusCode.OutOfRange, "Value too low"));
                        }

                        //Save into db
                        var reading = new MeterReading()
                        {
                            Value = r.ReadingValue,
                            ReadingDate = r.ReadingTime.ToDateTime(),
                            CustomerId = r.CustomerId
                        };

                        _repository.AddEntity(reading);

                        if (await _repository.SaveAllAsync())
                        {
                            _logger.LogInformation($"{request.Readings.Count} New Readings ...");
                            result.Success = ReadingStatus.Success;
                        }
                    }
                }
                catch (RpcException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    //result.Message = e.Message;
                    _logger.LogError($"Exception thrown during saving of readings:{e}");
                    throw  new RpcException(Status.DefaultCancelled, "Exception thrown during process");
                }
            }


            //return base.AddReading(request, context);
            return result;
        }
    }
}
