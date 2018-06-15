using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Taxys.Auth.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
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
        public Task<string> GetAsync(int valueId)
        {
            var value = Dataset[valueId];

            return Task.FromResult(value);
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
