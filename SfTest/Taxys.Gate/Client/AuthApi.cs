using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Taxys.Gate.Client
{
    public class AuthApi : IAuthApi
    {
        private readonly Uri baseUri;

        public AuthApi(Uri baseUri)
        {
            this.baseUri = baseUri;
        }

        public async Task<HttpResponseMessage> GetValueAsync(int valueId)
        {
            using (var httpClient = new HttpClient())
            {
                var uri = new Uri(baseUri, $"api/values/{valueId}");
                return await httpClient.GetAsync(uri);                
            }
        }
    }
}
