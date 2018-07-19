using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Taxys.Auth.Models;

namespace Taxys.Auth.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger<ValuesController> logger;
        private static int _maxKey;
        private static ConcurrentDictionary<int, string> _dataset;

        public ValuesController(ILogger<ValuesController> logger)
        {
            this.logger = logger;
            if (_dataset == null)
            {
                _dataset = new ConcurrentDictionary<int, string>(new Dictionary<int, string>
                {
                    { 1, "auth1" },
                    { 2, "auth2" }
                });

                _maxKey = _dataset.Keys.Max();
            }
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _dataset.Values;
        }

        // GET api/values/5
        [HttpGet("{valueId}")]
        public Task<IdValue> GetAsync(int valueId)
        {
            var exception = new NotImplementedException("BlaBla");
            throw exception;

            //logger.LogError(exception, "blabla");
            //var value = _dataset[valueId];

            //return Task.FromResult(new IdValue { Id = valueId, Value = value });
        }

        // POST api/values
        [HttpPost]
        public async Task<string> PostAsync([FromBody]string value)
        {
            var key = Interlocked.Increment(ref _maxKey);

            while (!_dataset.TryAdd(key, value))
                await Task.Delay(1000);
            
            return value;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public Task<string> PutAsync(int valueId, [FromBody]string value)
        {
            return Task.FromResult(_dataset.GetOrAdd(valueId, value));
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
