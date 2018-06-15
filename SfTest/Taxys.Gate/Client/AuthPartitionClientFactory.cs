using System;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace Taxys.Gate.Client
{
    internal class AuthPartitionClientFactory : IPartitionClientFactory<CommunicationClient<IAuthApi>>
    {
        private readonly ICommunicationClientFactory<CommunicationClient<IAuthApi>> factory;
        private readonly Uri serviceUri;

        public AuthPartitionClientFactory(ICommunicationClientFactory<CommunicationClient<IAuthApi>> factory, Uri serviceUri)
        {
            this.factory = factory;
            this.serviceUri = serviceUri;
        }

        public ServicePartitionClient<CommunicationClient<IAuthApi>> CreatePartitionClient() => 
            new ServicePartitionClient<CommunicationClient<IAuthApi>>(factory, serviceUri);

        public ServicePartitionClient<CommunicationClient<IAuthApi>> CreatePartitionClient(ServicePartitionKey partitionKey) => 
            new ServicePartitionClient<CommunicationClient<IAuthApi>>(factory, serviceUri, partitionKey);
    }
}
