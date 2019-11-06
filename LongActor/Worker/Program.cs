namespace Worker
{
    using System;
    using System.Threading;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<Book>();
            return services;
        }
    }
}
