using System.Net.Http;
using System.Threading.Tasks;

namespace Taxys.Gate.Client
{
    public interface IAuthApi
    {
        Task<HttpResponseMessage> GetValueAsync(int valueId);
    }
}
