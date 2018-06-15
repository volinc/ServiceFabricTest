using System;
using System.Net;
using System.Threading.Tasks;

namespace Taxys.Gate.Client
{
    public class AuthValuesRemoteController
    {
        private readonly IPartitionClientFactory<CommunicationClient<IAuthValuesApi>> factory;

        public AuthValuesRemoteController(IPartitionClientFactory<CommunicationClient<IAuthValuesApi>> factory)
        {
            this.factory = factory;
        }

        public async Task<string> GetValueAsync(int id)
        {
            var result = await factory.CreatePartitionClient()
            .InvokeWithRetryAsync(async client =>
            {
                var api = await client.CreateApiClient();
                return await api.GetValue(id);
            });

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException($"Not found {id}");
            }

            return await result.Content.ReadAsStringAsync();
        }
    }
}
