using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Binateq.JsonRestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Taxys.Gate.Remotes;

namespace Taxys.Gate.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly StatelessServiceContext serviceContext;
        private readonly ILogger<ValuesController> logger;
        private readonly string reverseProxyBaseUri;

        public ValuesController(HttpClient httpClient, StatelessServiceContext serviceContext, FabricClient fabricClient, ILogger<ValuesController> logger)
        {
            this.httpClient = httpClient;
            this.serviceContext = serviceContext;
            this.logger = logger;

            reverseProxyBaseUri = "http://localhost:19081"; // default value for local cluster
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {           
            logger.LogTrace("1 Getting all values");
            logger.LogDebug("2 Getting all values");
            logger.LogInformation("3 Getting all values");
            logger.LogWarning("4 Getting all values");
            logger.LogError("5 Getting all values");
            logger.LogCritical("6 Getting all values");

            var resolver = ServicePartitionResolver.GetDefault();
            var cancellationToken = CancellationToken.None;                        
            var partition = await resolver.ResolveAsync(new Uri("fabric:/SfTest/Taxys.Auth"), new ServicePartitionKey(), cancellationToken);   
            var endpoint = partition.GetEndpoint();
                
            var addresses = JObject.Parse(endpoint.Address);
            var address = (string)addresses["Endpoints"].First();

            if (!address.EndsWith("/"))
                address += "/";

            var client = new JsonRestClient(httpClient, new Uri(address));

            var content = await client.GetAsync<string[]>($"api/values", cancellationToken);
                        
            return content;
        }

        // GET api/values/5
        [HttpGet("{valueId}")]
        public async Task<string> GetAsync(int valueId)
        {
            var serviceName = GetServiceName("Taxys.Auth/");
            var proxyAddress = GetProxyAddress(serviceName);            

            var client = new JsonRestClient(httpClient, proxyAddress);
            var content = await client.GetAsync<IdValue>($"api/values/{valueId}");
            return content.Value;
        }

        [HttpGet("{valueId}/low-level")]
        public async Task<string> GetLowLevelAsync(int valueId)
        {
            var serviceName = GetServiceName("Taxys.Auth/");
            var proxyAddress = GetProxyAddress(serviceName);

            var proxyUrl = $"{proxyAddress}/api/values/{valueId}";
            if (serviceName.AbsoluteUri.EndsWith("Dispatcher"))
                proxyUrl += "?PartitionKey=1&PartitionKind=Int64Range";

            var httpResponse = await httpClient.GetAsync(proxyUrl);
            var content = await httpResponse.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<IdValue>(content);

            return result.Value;
        }
        
        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<string> PutAsync(int valueId, [FromBody]string value)
        {
            var serviceName = GetServiceName("Taxys.Auth/");
            var proxyAddress = GetProxyAddress(serviceName);

            var client = new JsonRestClient(httpClient, proxyAddress);
            var content = await client.PutAsync<string>($"api/values/{valueId}", value);
            return content;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private Uri GetServiceName(string name) =>
            new Uri($"{serviceContext.CodePackageActivationContext.ApplicationName}/{name}");

        private Uri GetProxyAddress(Uri serviceName) => 
            new Uri($"{reverseProxyBaseUri}{serviceName.AbsolutePath}");
    }
}
