using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json.Linq;
using Taxys.Gate.Client;

namespace Taxys.Gate.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly AuthValuesRemoteController authValuesRemoteController;

        public ValuesController(HttpClient httpClient, AuthValuesRemoteController authValuesRemoteController)
        {
            this.httpClient = httpClient;
            this.authValuesRemoteController = authValuesRemoteController;
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
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return authValuesRemoteController.GetValueAsync(id).Result;
        }

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
