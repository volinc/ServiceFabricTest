namespace Worker
{
    using System;
    using System.Threading.Tasks;
    using Worker.Interfaces;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    public class WorkerActor : Actor, IRemindable, IWorkerActor
    {
        private const string StateName = "state";
        private const string CountName = "count";
        private const string ReminderName = "worker";

        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<WorkerActor> logger;
        
        public WorkerActor(ActorService actorService, ActorId actorId, IServiceScopeFactory serviceScopeFactory, ILogger<WorkerActor> logger) 
            : base(actorService, actorId)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }                
        
        protected override async Task OnActivateAsync()
        {
            //ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            logger.LogInformation("Actor activated.");            

            var conditionalState = await StateManager.TryGetStateAsync<bool>(StateName);
            if (conditionalState.HasValue && conditionalState.Value)
                await RegisterReminderAsync(ReminderName, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }

        protected override async Task OnDeactivateAsync()
        {
            //ActorEventSource.Current.ActorMessage(this, "Actor deactivated.");
            logger.LogInformation("Actor deactivated.");        

            await StateManager.SaveStateAsync();
        }

        async Task<int> IWorkerActor.GetLongOpProgressAsync()
        {
            var conditional = await StateManager.TryGetStateAsync<int>(CountName);
            return conditional.HasValue ? conditional.Value : default;
        }

        async Task IWorkerActor.StartLongOpAsync()
        {
            var contionalState = await StateManager.TryGetStateAsync<bool>(StateName);
            if (contionalState.HasValue && contionalState.Value)
                return;

            await RegisterReminderAsync(ReminderName, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
            await StateManager.AddOrUpdateStateAsync(StateName, true, (key, value) => true);            
        }
                
        async Task IWorkerActor.StopLongOnAsync()
        {
            await UnregisterReminderAsync(GetReminder(ReminderName));
            await StateManager.AddOrUpdateStateAsync(StateName, false, (key, value) => false);
        }        

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {            
            using var scope = serviceScopeFactory.CreateScope();
            var book = scope.ServiceProvider.GetRequiredService<Book>();
            
            var message = book.ToString();
            logger.LogInformation(message);

            await StateManager.AddOrUpdateStateAsync(CountName, 1, (key, value) => ++value);            
            await RegisterReminderAsync(ReminderName, null, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1));
        }        
    }
}
