using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using MeterReaderWeb.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MeterReaderClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly ReadingFactory _factory;
        private readonly ILoggerFactory _loggerFactory;
        private string _token;
        private DateTime _expiration = DateTime.MinValue;
        private MeterReadingService.MeterReadingServiceClient _client = null;

        protected bool NeedsLogin() => string.IsNullOrEmpty(_token) || _expiration < DateTime.UtcNow;

        protected MeterReadingService.MeterReadingServiceClient Client
        {
            get
            {

                if (_client == null)
                {

                    //Step 3 : config certificate
                    //to enable a certificate we've got to use a handler 
                    var handler = new HttpClientHandler();
                    handler.ClientCertificates.Add(
                        new X509Certificate2(fileName:_configuration["Certificate:Filename"],
                                            password:_configuration["Certificate:Password"]));
                    //channel shouldn't create its own HttpClient but use ours
                    //TODO: IHttpClientFactory instead (Socket exhaustion PB!), here we use it only once in get property
                    var client = new HttpClient(handler);

                    var channel = GrpcChannel.ForAddress(_configuration.GetValue<string>("Service:ServerUrl"),
                        new GrpcChannelOptions()
                        {
                            HttpClient = client,
                            LoggerFactory = _loggerFactory
                        });
                    _client = new MeterReadingService.MeterReadingServiceClient(channel);
                }
                return _client;
            }
        }

        public Worker(ILogger<Worker> logger, IConfiguration configuration,
                      ReadingFactory factory, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _factory = factory;
            _loggerFactory = loggerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var counter = 0;
            var customerId = _configuration.GetValue<int>("Service:CustomerId");
            while (!stoppingToken.IsCancellationRequested)
            {

                counter++;
                //if (counter % 10 == 0)
                //{
                //    Console.WriteLine("Sending Diagnostics");
                //    var stream = Client.SendDiagnostics();
                //    for (int x = 0; x < 5; x++)
                //    {
                //        var reading = await _factory.Generate(customerId);
                //        await stream.RequestStream.WriteAsync(reading);
                //    }

                //    //we finish writing to you
                //    await stream.RequestStream.CompleteAsync();
                //}
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);



                var pkt = new ReadingPacket()
                {
                    Successful = ReadingStatus.Success,
                    Notes = "this a test"
                };


                for (int x = 0; x < 5; x++)
                {
                    pkt.Readings.Add(await _factory.Generate(customerId));
                }

                try
                {
                    ////used only for JWT authentication 
                    //if (!NeedsLogin() || await GenerateToken())
                    //{
                    //    var headers = new Metadata();
                    //    headers.Add("Authorization", $"Bearer {_token}");

                    //var result = await Client.AddReadingAsync(pkt, headers: headers);

                   var result = await Client.AddReadingAsync(pkt);

                        if (result.Success == ReadingStatus.Success)
                        {
                            _logger.LogInformation("Successfully sent");

                        }
                        else
                        {
                            _logger.LogError("Failed to send");
                        }
                    //}
                }
                catch (RpcException e)
                {

                    if (e.StatusCode == StatusCode.OutOfRange)
                    {
                        _logger.LogError($"{e.Trailers}");
                    }
                    Console.WriteLine(e);
                    throw;
                }


                await Task.Delay(_configuration.GetValue<int>("Service:DelayInterval"), stoppingToken);
            }
        }

        private async Task<bool> GenerateToken()
        {
            var  request = new TokenRequest()
            {
                Username = _configuration["Service:Username"],
                Password = _configuration["Service:Password"]
            };

            var response = await Client.CreateTokenAsync(request);
            if (response.Success)
            {
                _token = response.Token;
                _expiration = response.Expiration.ToDateTime();

                return true;
            }
            return false;
        }
    }
}
