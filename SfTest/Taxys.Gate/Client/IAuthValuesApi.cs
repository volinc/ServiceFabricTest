using System.Net.Http;
using System.Threading.Tasks;

namespace Taxys.Gate.Client
{
    public interface IAuthValuesApi
    {
        Task<HttpResponseMessage> GetValue(int id);
    }
}
