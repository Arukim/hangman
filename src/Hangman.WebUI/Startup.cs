using Hangman.Core;
using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using Hangman.Persistence;
using Hangman.WebUI.Consumers;
using Hangman.WebUI.Controllers;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hangman.WebUI
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
            services.AddSignalR();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddPersistence(Configuration);
            services.AddMessaging(Configuration);


            var rmqSection = Configuration.GetSection(Constants.ConfigSections.Rabbit);
            var rmqConfig = rmqSection.Get<RabbitMQConfiguration>();

            // Hostname must be unique for each instance of WebUI
            // In docker it is set to container Id
            // In K8S it is set to pod Id
            var hostname = Environment.MachineName;

            services.AddMassTransit(x =>
            {
                x.AddConsumer<GameStatusConsumer>();

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri(rmqConfig.Endpoint), hostConfigurator =>
                    {
                        hostConfigurator.Username(rmqConfig.Username);
                        hostConfigurator.Password(rmqConfig.Password);
                    });

                    // Each instance of WebUI creates own exchange / queue pair
                    cfg.ReceiveEndpoint(host, $"webui-{hostname}", ep =>
                    {
                        // Delete exchange and queue 
                        // If WebUI instance is switched off
                        // Otherwise it would keep collecting messages
                        ep.AutoDelete = true;
                        // We don't care about persistance for this kind of events
                        // Non-durable queue is much faster
                        ep.Durable = false;
                        ep.Consumer<GameStatusConsumer>(provider);
                    });

                    cfg.ConfigureEndpoints(provider);
                }));
                                
                x.AddRequestClient<GameStatus>();
            });

            services.AddScoped<GameStatusConsumer>();


            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalRCounter>("/signalr");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            var bus = app.ApplicationServices.GetService<IBusControl>();
            bus.Start();
        }
    }
}
