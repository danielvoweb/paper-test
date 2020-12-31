using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiClient.Clients
{
    public class BaconIpsumApiClient : IApiClient<string>
    {
        private readonly HttpClient _httpClient;
        public BaconIpsumApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BaconClient");
        }
        
        public async Task<IEnumerable<string>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("?type=meat-and-filler", cancellationToken);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string[]>(content);
        }

        public async Task<string> GetAsync(string identifier, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}