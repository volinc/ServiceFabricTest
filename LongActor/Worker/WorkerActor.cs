namespace Worker
{
    using System;
    using System.Threading.Tasks;
    using global::Worker.Interfaces;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;

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

        public WorkerActor(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {            
        }                
        
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            var conditionalState = await StateManager.TryGetStateAsync<bool>(StateName);
            if (conditionalState.HasValue && conditionalState.Value)
                await RegisterReminderAsync(ReminderName, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }

        protected override async Task OnDeactivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor deactivated.");            
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
            await StateManager.AddOrUpdateStateAsync(CountName, 1, (key, value) => ++value);            
            await RegisterReminderAsync(ReminderName, null, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1));
        }        
    }
}
