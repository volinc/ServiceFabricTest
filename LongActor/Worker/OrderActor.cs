namespace Worker
{
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Worker.Interfaces;
    using System;
    using System.Collections.Generic;

    [StatePersistence(StatePersistence.Persisted)]
    public class OrderActor : Actor, IOrderActor
    {
        public OrderActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }
    }

    public class OrderData
    {
        public long Id { get; set; }
        public Guid Uuid { get; set; }
        public long PassengerId { get; set; } 
        public string HandlerName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? SearchingStartAt { get; set; }
        public DateTimeOffset? SearchingEndAt { get; set; }        
        public DateTimeOffset? ExpiredAt { get; set; }                
        public long? SuggestionId { get; set; }
        public SuggestionData Suggestion { get; set; }
        public List<SuggestionData> Suggestions { get; set; } = new List<SuggestionData>();        
    }

    public class SuggestionData
    {        
        public long Id { get; set; }

        public long OrderId { get; set; }
    }
}
