// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FcmSharp.Scheduler.Quartz.Quartz.Jobs;
using FcmSharp.Scheduler.Quartz.Services;
using FcmSharp.Scheduler.Quartz.Testing;
using FcmSharp.Scheduler.Quartz.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FcmSharp.Scheduler.Quartz
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; set; }

        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            Environment = env;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .Build();
        }

        // Este método es llamado por el tiempo de ejecución. Utilice este método para agregar servicios al contenedor.
        public void ConfigureServices(IServiceCollection services)
        {
            // agregar una politica CORS para permitir todo. CORS permite controlar las peticiones HTTP asíncronas
            // que se pueden realizar desde un navegador a un servidor con un dominio diferente de la página cargada originalmente
            services.AddCors(o =>
            {
                o.AddPolicy("Everything", p =>
                {
                    p.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });

            services
                .AddOptions()
                .AddQuartz()
                .AddTransient<ProcessMessageJob>()
                .AddTransient<IFcmClient, MockFcmClient>()
                .AddTransient<ISchedulerService, SchedulerService>()
                .AddTransient<IMessagingService, MessagingService>()
                .AddMvc();
        }

        // Este método se llama en tiempo de ejecución. Con este método configuramos la canalización de solicitudes HTTP.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.EnsureDatabaseCreated()
               .UseCors("Everything")
               .UseStaticFiles()
               .UseQuartz()
               .UseMvc();
        }
    }
}