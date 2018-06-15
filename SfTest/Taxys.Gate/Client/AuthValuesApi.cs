using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Taxys.Gate.Client
{
    public class AuthValuesApi : IAuthValuesApi
    {
        private readonly Uri baseUri;

        public AuthValuesApi(Uri baseUri)
        {
            this.baseUri = baseUri;
        }

        public async Task<HttpResponseMessage> GetValue(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var uri = new Uri(baseUri, $"api/values/{id}");
                return await httpClient.GetAsync(uri);                
            }
        }
    }
}
