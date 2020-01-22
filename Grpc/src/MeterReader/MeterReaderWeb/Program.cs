using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MeterReaderLib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MeterReaderWeb
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            { 
              webBuilder.UseStartup<Startup>();

              //step 1 : config certificate on kestrel
              webBuilder.ConfigureKestrel(opt =>
                  {
                      opt.ConfigureHttpsDefaults(configureOptions: h =>
                          {
                              //accepts certificates and no certificate request
                              h.ClientCertificateMode = ClientCertificateMode.AllowCertificate;

                              //Don't check the certificate if signed by a trusted authority (self-signed for dev)
                              h.CheckCertificateRevocation = false;

                              var config = opt.ApplicationServices.GetService<IConfiguration>();

                              //configure the ServerCertificate
                              h.ServerCertificate = new X509Certificate2(fileName: config["Certificate:Filename"],
                                  password:config["Certificate:Password"]);
                          });
                  }
              );
            });
  }
}
