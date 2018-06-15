using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Taxys.Gate.Client;

namespace Taxys.Gate
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
            services.AddMvc();                       
            services.AddSingleton<HttpClient>();

            services.AddSingleton(new Uri("fabric:/SfTest/Taxys.Auth"));
            services.AddSingleton<FabricClient>();

            services.AddSingleton<ICommunicationClientFactory<CommunicationClient<IAuthApi>>>(
            serviceProvider => new AuthCommunicationClientFactory(
                new ServicePartitionResolver(() => serviceProvider.GetService<FabricClient>())));

            services.AddSingleton<IPartitionClientFactory<CommunicationClient<IAuthApi>>, AuthPartitionClientFactory>();

            services.AddScoped<AuthValuesRemoteController>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
