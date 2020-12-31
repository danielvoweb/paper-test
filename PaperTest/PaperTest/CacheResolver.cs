using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaperTest
{
    public interface ICacheResolver
    {
        Task<(bool cached, HttpResponseMessage response)> BeforeRequest(HttpRequestMessage request);

        Task<HttpResponseMessage> AfterRequest(HttpResponseMessage response);
    }

    public class CacheResolver : ICacheResolver
    {
        private readonly IRepository<PaperEndpoint> _repository;
        private readonly ISettings _settings;
        public CacheResolver(IRepository<PaperEndpoint> repository, ISettings settings)
        {
            _repository = repository;
            _settings = settings;
        }
        
        public async Task<(bool cached, HttpResponseMessage response)> BeforeRequest(HttpRequestMessage request)
        {
            var existing = await _repository.Get(request.ThatMatches());
            if (existing == null) return (false, null);

            var cached = existing.TimeToLive > DateTime.Now;
            if (!cached) await _repository.Remove(x => x.Id == existing.Id);

            return (cached, existing.ToHttpResponseMessage(request.Headers));
        }

        public async Task<HttpResponseMessage> AfterRequest(HttpResponseMessage response)
        {
            var endpoint = await response.ToPaperEndpoint(_settings.SecondsToLive);
            var savedEndpoint = await _repository.Create(endpoint);
            return savedEndpoint.ToHttpResponseMessage(response.Headers, response.TrailingHeaders);
        }
    }
}