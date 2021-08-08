using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using MongoDB.Driver;
using EventStore.Client;
using Eventuous;
using Eventuous.Subscriptions;
using Eventuous.Subscriptions.EventStoreDB;
using Eventuous.Projections.MongoDB;
using Eventuous.EventStoreDB;

using Eventuous.Sample.Application;
using Eventuous.Sample.Domain;
using Eventuous.Sample.Infrastructure;
using Eventuous.Sample.Application.Projections;
using Eventuous.Sample.Application.Reactions;
using Eventuous.Sample.Application.Queries;

namespace Eventuous.Sample
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "eventuous.reaction", Version = "v1" });
            });

            services
                .AddEventStore(Configuration["EventStore"])
                .AddMongoStore(Configuration["MongoDB"])
                .AddCustomServices()
                .AddReactions()
                .AddProjections()
            ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "eventuous.reaction v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class StartupExtensions
    {
        public static IServiceCollection AddCustomServices(
            this IServiceCollection services
        )
        {
            services
                .AddSingleton<IExternalService, ExternalService>()
                .AddSingleton<WidgetCommandService>()
                .AddSingleton<WidgetQueryService>()
                ;
            return services;
        }

        public static IServiceCollection AddProjections(
            this IServiceCollection services)
        {
            services
                .AddSingleton<IHostedService, AllStreamSubscription>( provider => {
                    var subscriptionId = "projections";
                    var loggerFactory = provider.GetLoggerFactory();

                    return new AllStreamSubscription(
                        provider.GetEventStoreClient(),
                        subscriptionId,
                        new MongoCheckpointStore(
                            provider.GetMongoDatabase(),
                            loggerFactory.CreateLogger<MongoCheckpointStore>()
                        ),
                        new IEventHandler[] { 
                            new WidgetProjector(
                                provider.GetMongoDatabase(), 
                                subscriptionId, 
                                loggerFactory
                            )
                        },
                        DefaultEventSerializer.Instance,
                        loggerFactory
                    );

                });

            return services;
        }        

        public static IServiceCollection AddReactions(
            this IServiceCollection services)
        {
            services
                .AddSingleton<IHostedService, AllStreamSubscription>( provider => {
                    var subscriptionId = "reactions";
                    var loggerFactory = provider.GetLoggerFactory();

                    return new AllStreamSubscription(
                        provider.GetEventStoreClient(),
                        new AllStreamSubscriptionOptions() {
                            SubscriptionId = subscriptionId,
                            ThrowOnError = true
                        },
                        new MongoCheckpointStore(
                            provider.GetMongoDatabase(),
                            loggerFactory.CreateLogger<MongoCheckpointStore>()
                        ),
                        new IEventHandler[] { 
                            new WidgetReactor(
                                subscriptionId, 
                                provider.GetRequiredService<WidgetCommandService>()
                            )
                        },
                        DefaultEventSerializer.Instance,
                        loggerFactory
                    );
                });
            return services;
        }

        public static IServiceCollection AddEventStore(
            this IServiceCollection services,
            string eventStoreConnectionString
        )
        {
            EventMapping.MapEventTypes();
            
            var settings = EventStoreClientSettings.Create(eventStoreConnectionString);
            var eventStoreClient = new EventStoreClient(settings);
            var eventStore = new EsdbEventStore(eventStoreClient);
            services.AddSingleton(eventStoreClient);
            var aggregateStore = new AggregateStore(eventStore, DefaultEventSerializer.Instance);
            services.AddSingleton<IAggregateStore>(aggregateStore);
            return services;
        }

        public static IServiceCollection AddMongoStore(
            this IServiceCollection services,
            string mongoDBConnectionString
        )
        {
            var mongoClient = new MongoClient(mongoDBConnectionString);
            var database = mongoClient.GetDatabase("readside");
            services.AddSingleton<IMongoDatabase>(database);
            return services;
        }

        public static ILoggerFactory GetLoggerFactory(this IServiceProvider provider)
            => provider.GetRequiredService<ILoggerFactory>();
        public static IAggregateStore GetAggregateStore(this IServiceProvider provider)
            => provider.GetRequiredService<IAggregateStore>();
        public static IMongoDatabase GetMongoDatabase(this IServiceProvider provider)
            => provider.GetRequiredService<IMongoDatabase>();
        public static EventStoreClient GetEventStoreClient(this IServiceProvider provider)
            => provider.GetRequiredService<EventStoreClient>();

    }    
}
