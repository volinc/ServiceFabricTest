using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json.Linq;

namespace Taxys.Gate.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly StatelessServiceContext serviceContext;
        private readonly FabricClient fabricClient;
        private readonly string reverseProxyBaseUri;

        public ValuesController(HttpClient httpClient, StatelessServiceContext serviceContext, FabricClient fabricClient)
        {
            this.httpClient = httpClient;
            this.serviceContext = serviceContext;
            this.fabricClient = fabricClient;

            reverseProxyBaseUri = "http://localhost:19081"; // default value for local cluster
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {                      
            var resolver = ServicePartitionResolver.GetDefault();
            var cancellationToken = CancellationToken.None;            
            var serviceUri = new Uri("fabric:/SfTest/Taxys.Auth");
            var partition = resolver.ResolveAsync(serviceUri, new ServicePartitionKey(), cancellationToken).Result;    
            var endpoint = partition.GetEndpoint();
                
            var addresses = JObject.Parse(endpoint.Address);
            var address = (string)addresses["Endpoints"].First();

            var httpResponse = httpClient.GetAsync($"{address}/api/values").Result;
            
            var content = httpResponse.Content.ReadAsAsync<IEnumerable<string>>().Result;
            return content;
        }

        // GET api/values/5
        [HttpGet("{valueId}")]
        public async Task<string> GetAsync(int valueId)
        {
            var serviceName = GetAuthServiceName();
            var proxyAddress = GetProxyAddress(serviceName);
 
            var proxyUrl = $"{proxyAddress}/api/values/{valueId}";
            var httpResponse = await httpClient.GetAsync(proxyUrl);
            
            var content = await httpResponse.Content.ReadAsStringAsync();
            return content;
        }

        private Uri GetAuthServiceName() =>
            new Uri($"{serviceContext.CodePackageActivationContext.ApplicationName}/Taxys.Auth");

        private Uri GetProxyAddress(Uri serviceName) => 
            new Uri($"{reverseProxyBaseUri}{serviceName.AbsolutePath}");

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
