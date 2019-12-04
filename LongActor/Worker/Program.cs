namespace Worker
{
    using System;
    using System.Fabric;
    using System.Threading;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.ServiceFabric;
    using Microsoft.ApplicationInsights.ServiceFabric.Module;
    using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.ApplicationInsights;

    internal static class Program
    {
        private static void Main()
        {
            try
            {
                var config = new ConfigurationBuilder()
                             .AddEnvironmentVariables()
                             .Build();

                var services = ConfigureServices(config);

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterActor<WorkerActor>();                
                builder.Populate(services);

                using (builder.Build())
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception exception)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(exception.ToString());
                throw;
            }
        }

        private static ServiceCollection ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            //var channel = new InMemoryChannel();
            var channel = new ServerTelemetryChannel();
            services.Configure<TelemetryConfiguration>(options =>
            {                
                options.TelemetryChannel = channel;
                options.TelemetryInitializers.Add(new FabricTelemetryInitializer());                
            });

            services.AddSingleton<ITelemetryModule>(new ServiceRemotingDependencyTrackingTelemetryModule())
                    .AddSingleton<ITelemetryModule>(new ServiceRemotingRequestTrackingTelemetryModule());            

            services.AddLogging(options =>
            {                
                var instrumentationKey = configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");
                options.AddApplicationInsights(instrumentationKey)
                       .AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Trace);

                //options.AddDebug();
                //options.SetMinimumLevel(LogLevel.Trace);
            });

            services.AddScoped<Book>();
            return services;
        }        
    }
}
