using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiClient.Clients
{
    public interface IApiClient<T>
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
        Task<T> GetAsync(string identifier, CancellationToken cancellationToken);
    }
}