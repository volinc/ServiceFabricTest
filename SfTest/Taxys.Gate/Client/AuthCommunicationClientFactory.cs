using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Taxys.Gate.Client
{

    internal class AuthCommunicationClientFactory : CommunicationClientFactoryBase<CommunicationClient<IAuthApi>>
    {
        public AuthCommunicationClientFactory(IServicePartitionResolver resolver)
            : base(resolver, new[] { new HttpExceptionHandler() })
        {
        }

        protected override void AbortClient(CommunicationClient<IAuthApi> client)
        {
            throw new NotImplementedException();
        }

        protected override Task<CommunicationClient<IAuthApi>> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
        {
            var client = new CommunicationClient<IAuthApi>(
                () => Task.FromResult<IAuthApi>(new AuthApi(new Uri(endpoint))));

            return Task.FromResult(client);
        }

        protected override bool ValidateClient(CommunicationClient<IAuthApi> client)
        {
            throw new NotImplementedException();
        }

        protected override bool ValidateClient(string endpoint, CommunicationClient<IAuthApi> client)
        {
            throw new NotImplementedException();
        }
    }    
}
