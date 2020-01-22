using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MeterReaderLib;
using MeterReaderWeb.Data;
using MeterReaderWeb.Services;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MeterReaderWeb
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<JwtTokenValidationService>();
            services.AddAuthentication()
              .AddJwtBearer(cfg =>
              {
                  cfg.TokenValidationParameters = new MeterReaderTokenValidationParameters(_config);
              })
              //Step 2 : config certificate
              .AddCertificate(opt =>
              {
                  opt.AllowedCertificateTypes = CertificateTypes.SelfSigned;
                  opt.RevocationMode = X509RevocationMode.NoCheck; //Just for dev

                  //use to allow a call back on performing a validation for instance...
                  opt.Events = new CertificateAuthenticationEvents()
                  {
                      //I've done all my validations, go ahead and mark it as success
                      OnCertificateValidated = context =>
                      {
                          context.Success(); 
                          return  Task.CompletedTask;
                      }
                  };
              });


            services.AddDbContext<ReadingContext>(options =>
                options.UseSqlServer(
                    _config.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
              .AddEntityFrameworkStores<ReadingContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            //add all required services for grpc.
            services.AddGrpc(opt =>
            {
                opt.EnableDetailedErrors = true;
                opt.MaxReceiveMessageSize = Int32.MaxValue;
            });

            services.AddScoped<IReadingRepository, ReadingRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
         
            app.UseEndpoints(endpoints =>
            {
                //IIS Express and IIS. And even AZURE APP service don't yet support HTTP/2, because of an underlying component
                //called HTTP.sys doesn't yet support the new protocol of HTTP/2
                //We Can't host gRPC in IIS/Azure App Service. The HTTP/2 implementation of Http.Sys does not support
                //HTTP response trailing headers which gRPC
                //So we need to run it as a Kestral Web server.
                endpoints.MapGrpcService<MeterService>();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
