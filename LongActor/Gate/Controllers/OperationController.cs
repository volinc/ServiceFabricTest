namespace Gate.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Worker.Interfaces;

    [ApiController]
    [Route("operation")]
    public class OperationController : ControllerBase
    {
        private readonly ActorProvider actorProvider;
        private readonly ILogger<OperationController> logger;

        public OperationController(ActorProvider actorProvider, 
                                   ILogger<OperationController> logger)
        {
            this.actorProvider = actorProvider;
            this.logger = logger;
        }

        [HttpGet]
        public Task<int> GetLongOpProgressAsync()
        {
            return GetWorker().GetLongOpProgressAsync();
        }

        [HttpPost("start")]
        public Task StartLongOpAsync()
        {
            GetWorker().StartLongOpAsync().FireAndForget(logger);

            return Task.CompletedTask;
        }

        [HttpPost("stop")]
        public async Task StopLongOpAsync()
        {            
            await GetWorker().StopLongOnAsync();
        }

        [HttpPost("force-stop")]
        public async Task ForceStopLongOpAsync()
        {
            var uuid = Guid.Parse("D544F38E20DF4826AB92FE10C5D1388C");
            await actorProvider.DeleteWorkerAsync(uuid, CancellationToken.None);
        }

        private IWorkerActor GetWorker()
        {
            var uuid = Guid.Parse("D544F38E20DF4826AB92FE10C5D1388C");
            var worker = actorProvider.GetWorker(uuid);
            return worker;
        }
    }
}
