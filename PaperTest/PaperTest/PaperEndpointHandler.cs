using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PaperTest
{
    public class PaperEndpointHandler : DelegatingHandler
    {
        private readonly ICacheResolver _cacheResolver;
        private readonly ILogger _logger;

        public PaperEndpointHandler(
            ICacheResolver cacheResolver,
            ILogger<PaperEndpointHandler> logger)
        {
            _cacheResolver = cacheResolver;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var (cached, response) = await _cacheResolver.BeforeRequest(request);
            if (cached)
            {
                _logger.LogInformation($"Found cached HTTP response for {response?.RequestMessage?.Method} {response?.RequestMessage?.RequestUri}");
                return response;
            }
            response = await base.SendAsync(request, cancellationToken);
            var storedResponse = await _cacheResolver.AfterRequest(response);
            _logger.LogInformation($"Stored cached HTTP response for {storedResponse?.RequestMessage?.Method} {storedResponse?.RequestMessage?.RequestUri}");
            return storedResponse;
        }
    }
}