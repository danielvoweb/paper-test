using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PaperTest
{
    public static class Extensions
    {
        public static Func<PaperEndpoint, bool> ThatMatches(this HttpRequestMessage requestMessage)
        {
            if (requestMessage == null) return _ => false;
            return x => requestMessage.RequestUri == x.Endpoint && requestMessage.Method == x.Method;
        }

        public static async Task<PaperEndpoint> ToPaperEndpoint(this HttpResponseMessage responseMessage,
            int secondsToLive)
        {
            if (responseMessage.RequestMessage == null) return new PaperEndpoint();

            var content = await responseMessage.Content.ReadAsStringAsync();

            return new PaperEndpoint
            {
                Endpoint = responseMessage.RequestMessage.RequestUri,
                Method = responseMessage.RequestMessage.Method,
                StatusCode = responseMessage.StatusCode,
                ReasonPhase = responseMessage.ReasonPhrase,
                Headers = JsonConvert.SerializeObject(responseMessage.Headers),
                TrailingHeaders = JsonConvert.SerializeObject(responseMessage.TrailingHeaders),
                Content = content,
                TimeToLive = DateTime.Now.AddSeconds(secondsToLive),
            };
        }

        public static HttpResponseMessage ToHttpResponseMessage(
            this PaperEndpoint endpoint,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> trailingHeaders = null)
        {
            var message = new HttpResponseMessage(endpoint.StatusCode)
            {
                RequestMessage = new HttpRequestMessage(endpoint.Method, endpoint.Endpoint),
                ReasonPhrase = endpoint.ReasonPhase
            };

            if (endpoint.Content != null) message.Content = new StringContent(endpoint.Content);
            if (headers == null) return message;
            foreach (var (key, value) in headers)
            {
                message.Headers.Add(key, value);
            }

            if (trailingHeaders == null) return message;
            foreach (var (key, value) in trailingHeaders)
            {
                message.TrailingHeaders.Add(key, value);
            }

            return message;
        }
    }
}