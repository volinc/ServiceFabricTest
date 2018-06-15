using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Taxys.Gate.Client
{

    internal class AuthValueCommunicationClientFactory : CommunicationClientFactoryBase<CommunicationClient<IAuthValuesApi>>
    {
        public AuthValueCommunicationClientFactory(IServicePartitionResolver resolver)
            : base(resolver, new[] { new HttpExceptionHandler() })
        {
        }

        protected override void AbortClient(CommunicationClient<IAuthValuesApi> client)
        {
            throw new NotImplementedException();
        }

        protected override Task<CommunicationClient<IAuthValuesApi>> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
        {
            var client = new CommunicationClient<IAuthValuesApi>(
                () => Task.FromResult<IAuthValuesApi>(new AuthValuesApi(new Uri(endpoint))));

            return Task.FromResult(client);
        }

        protected override bool ValidateClient(CommunicationClient<IAuthValuesApi> client)
        {
            throw new NotImplementedException();
        }

        protected override bool ValidateClient(string endpoint, CommunicationClient<IAuthValuesApi> client)
        {
            throw new NotImplementedException();
        }
    }    
}
