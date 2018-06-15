using System;
using System.Net;
using System.Threading.Tasks;

namespace Taxys.Gate.Client
{
    public class AuthValuesRemoteController
    {
        private readonly IPartitionClientFactory<CommunicationClient<IAuthApi>> factory;

        public AuthValuesRemoteController(IPartitionClientFactory<CommunicationClient<IAuthApi>> factory)
        {
            this.factory = factory;
        }

        public async Task<string> GetValueAsync(int valueId)
        {
            var result = await factory.CreatePartitionClient()
            .InvokeWithRetryAsync(async client =>
            {
                var api = await client.CreateApiClient();
                return await api.GetValueAsync(valueId);
            });

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException($"Not found {valueId}");
            }

            return await result.Content.ReadAsStringAsync();
        }
    }
}
