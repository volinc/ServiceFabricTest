using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Fabric;
using System.Threading.Tasks;

namespace Taxys.Gate.Client
{
    public class CommunicationClient<T> : ICommunicationClient
    {
        private readonly Func<Task<T>> apiFactory;

        public CommunicationClient(Func<Task<T>> apiFactory)
        {
            this.apiFactory = apiFactory;
        }

        public Task<T> CreateApiClient() => apiFactory();

        public ResolvedServiceEndpoint Endpoint { get; set; }

        public string ListenerName { get; set; }

        public ResolvedServicePartition ResolvedServicePartition { get; set; }
    }
}
