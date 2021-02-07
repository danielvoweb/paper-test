using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiClient.Clients.BaconIpsum
{
    /// <summary>
    /// HTTP Client for https://baconipsum.com/json-api/
    /// </summary>
    public class BaconIpsumApiClient
    {
        private readonly HttpClient _httpClient;
        public BaconIpsumApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BaconClient");
        }
        
        public async Task<IEnumerable<string>> GetAllAsync(BaconIpsumRequestObject requestObject, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(requestObject.RequestPart(), cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<string[]>(content);
        }
    }
}