namespace Taxys.Gate
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.ServiceFabric;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Gate : StatelessService
    {
        public Gate(StatelessServiceContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "EndpointHttps", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                            .UseKestrel(opt =>
                            {
                                var port = serviceContext.CodePackageActivationContext.GetEndpoint("EndpointHttps").Port;
                                opt.Listen(IPAddress.IPv6Any, port, listenOptions =>
                                {
                                    listenOptions.UseHttps(GetCertificateFromStore());
                                    listenOptions.NoDelay = true;
                                });
                            })
                            .ConfigureServices(services => services
                                .AddSingleton(serviceContext)
                                .AddSingleton<ITelemetryInitializer>(serviceProvider => FabricTelemetryInitializerExtension.CreateFabricTelemetryInitializer(serviceContext)))
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseStartup<Startup>()
                            .UseApplicationInsights()
                            .ConfigureAppConfiguration((hostContext, config) =>
                            {                                        
                                config.Sources.Clear();
                                config.AddEnvironmentVariables();
                            })   
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                            .UseUrls(url)
                            .Build();
                    }))
            };
        }

        private static X509Certificate2 GetCertificateFromStore()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates;
                var currentCerts = certCollection.Find(X509FindType.FindBySubjectDistinguishedName, "CN=localhost", false);
                return currentCerts.Count == 0 ? null : currentCerts[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}