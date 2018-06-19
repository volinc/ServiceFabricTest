using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Taxys.Auth.Models;

namespace Taxys.Auth.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static int maxKey;
        private static ConcurrentDictionary<int, string> Dataset;

        public ValuesController()
        {
            if (Dataset == null)
            {
                Dataset = new ConcurrentDictionary<int, string>(new Dictionary<int, string>
                {
                    { 1, "auth1" },
                    { 2, "auth2" }
                });

                maxKey = Dataset.Keys.Max();
            }
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Dataset.Values;
        }

        // GET api/values/5
        [HttpGet("{valueId}")]
        public Task<IdValue> GetAsync(int valueId)
        {
            var value = Dataset[valueId];

            return Task.FromResult(new IdValue { Id = valueId, Value = value });
        }

        // POST api/values
        [HttpPost]
        public async Task<string> PostAsync([FromBody]string value)
        {
            var key = Interlocked.Increment(ref maxKey);

            while (!Dataset.TryAdd(key, value))
                await Task.Delay(1000);
            
            return value;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public Task<string> PutAsync(int valueId, [FromBody]string value)
        {
            return Task.FromResult(Dataset.GetOrAdd(valueId, value));
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
