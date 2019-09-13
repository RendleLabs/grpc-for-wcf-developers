using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockService.Protos;
using StockWeb.Internal;
using StockWeb.Settings;

namespace StockWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
            services.AddControllersWithViews();
            services.Configure<StockServiceSettings>(Configuration.GetSection("StockService"));
            services.AddGrpcClient<Stocks.StocksClient>((provider, options) =>
                {
                    var logger = provider.GetRequiredService<ILogger<Stocks.StocksClient>>();
                    var settings = provider.GetRequiredService<IOptionsMonitor<StockServiceSettings>>();
                    logger.LogCritical("URL '{address}'", settings.CurrentValue.Address);
                    options.Address = new Uri(settings.CurrentValue.Address);
                });
//                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
//                {
//                    ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
//                })
//                .ConfigureChannel(channel => { channel.Credentials = ChannelCredentials.Insecure; });
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
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}