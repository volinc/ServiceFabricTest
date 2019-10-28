namespace Worker.Interfaces
{
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;
    using System;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class ActorProvider
    {
        private readonly ServiceContext serviceContext;
        private readonly Uri workerServiceUri;

        public ActorProvider(ServiceContext serviceContext)
        {
            this.serviceContext = serviceContext;
            workerServiceUri = new Uri($"{serviceContext.CodePackageActivationContext.ApplicationName}/WorkerActorService");
        }

        public ActorProvider(StatelessServiceContext serviceContext) : this ((ServiceContext)serviceContext)
        {
        }

        public ActorProvider(StatefulServiceContext serviceContext) : this((ServiceContext)serviceContext)
        {
        }        

        public IWorkerActor GetWorker(Guid uuid)
        {
            var actorId = new ActorId(uuid);
            return ActorProxy.Create<IWorkerActor>(actorId, workerServiceUri);            
        }

        public async Task DeleteWorkerAsync(Guid uuid, CancellationToken cancellationToken)
        {
            var actorId = new ActorId(uuid);
            var actorService = ActorServiceProxy.Create(workerServiceUri, actorId);
            var page = await actorService.GetActorsAsync(null, cancellationToken);
            if (page.Items.Any(x => x.ActorId == actorId && x.IsActive))
                await actorService.DeleteActorAsync(actorId, cancellationToken);
        }
    }
}
