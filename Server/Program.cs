using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Server
{
    class Program
    {
        // Microsoft.AspNetCore.Cors 2.2.0
        // Microsoft.AspNetCore.Owin 5.0.17
        // Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson 5.0.17

        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://127.0.0.1:9999");
                    webBuilder.UseStartup<Startup>();
                });
        }

        internal class Startup
        {

            public void ConfigureServices(IServiceCollection services)
            {
                // I way: Intergrated Json (2 times faster)
                //services.AddSignalR().AddJsonProtocol(o =>
                //{
                //    //o.PayloadSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                //    //o.PayloadSerializerOptions.WriteIndented = true;
                //});

                //Use in code:
                //Message createdMessage = JsonSerializer.Deserialize<Message>(jsonData);


                // II way: NewtonsoftJson => for circular dependencies and immutable types + value types

                services.AddSignalR().AddNewtonsoftJsonProtocol(o =>
                {
                    o.PayloadSerializerSettings.StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeHtml;
                    o.PayloadSerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All;
                    o.PayloadSerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                });

            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<ServerHub>("/test");
                });
            }
        }
    }
}
