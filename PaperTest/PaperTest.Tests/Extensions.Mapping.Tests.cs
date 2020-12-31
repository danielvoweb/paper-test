using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace PaperTest.Tests
{
    public class ExtensionsMappingTests
    {
        [Test]
        public async Task Should_MapToPaperEndpoint()
        {
            // Arrange
            const int SECONDS_TO_LIVE = 30;

            const string EXPECTED_CONTENT = "content";

            var expected = new HttpResponseMessage
            {
                RequestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://example.com"),
                    Method = HttpMethod.Get
                },
                StatusCode = HttpStatusCode.Accepted,
                ReasonPhrase = "OK",
                Content = new StringContent(EXPECTED_CONTENT)
            };

            expected.Headers.Add("key", "value");
            expected.TrailingHeaders.Add("key", "value");

            // Act
            var actual = await expected.ToPaperEndpoint(SECONDS_TO_LIVE);

            // Assert
            Assert.Multiple(() =>
            {
                actual.Endpoint.Should().Be(expected.RequestMessage?.RequestUri);
                actual.Method.Should().Be(expected.RequestMessage?.Method);
                actual.StatusCode.Should().Be(expected.StatusCode);
                actual.ReasonPhase.Should().Be(expected.ReasonPhrase);
                actual.Headers.Should().Be(JsonConvert.SerializeObject(expected.Headers));
                actual.TrailingHeaders.Should().Be(JsonConvert.SerializeObject(expected.TrailingHeaders));
                actual.Content.Should().Be(EXPECTED_CONTENT);
                actual.TimeToLive.Should().BeCloseTo(DateTime.Now.AddSeconds(SECONDS_TO_LIVE));
            });
        }

        [Test]
        public async Task Should_ReturnAnEmptyPaperEndpointIfResponseIsNull()
        {
            // Arrange
            const int SECONDS_TO_LIVE = 30;
            var expected = new HttpResponseMessage();

            // Act
            var actual = await expected.ToPaperEndpoint(SECONDS_TO_LIVE);

            // Assert
            actual.Should().BeEquivalentTo(new PaperEndpoint());
        }

        [Test]
        public void Should_MapToHttpResponseMessageFromResponse()
        {
            // Arrange
            var response = new HttpResponseMessage();
            response.Headers.Add("key", "value");
            response.TrailingHeaders.Add("key", "value");

            const string EXPECTED_CONTENT = "content";

            var expected = new PaperEndpoint
            {
                Endpoint = new Uri("http://example.com"),
                Method = HttpMethod.Get,
                StatusCode = HttpStatusCode.Accepted,
                ReasonPhase = "OK",
                Headers = JsonConvert.SerializeObject(response.Headers),
                TrailingHeaders = JsonConvert.SerializeObject(response.TrailingHeaders),
                Content = EXPECTED_CONTENT
            };

            // Act
            var actual = expected.ToHttpResponseMessage(response.Headers, response.TrailingHeaders);

            // Assert
            Assert.Multiple(async () =>
            {
                actual.RequestMessage?.RequestUri.Should().Be(expected.Endpoint);
                actual.RequestMessage?.Method.Should().Be(expected.Method);
                actual.StatusCode.Should().Be(expected.StatusCode);
                actual.ReasonPhrase.Should().Be(expected.ReasonPhase);
                actual.Headers.Should()
                    .BeEquivalentTo(
                        JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, IEnumerable<string>>>>(
                            expected.Headers));
                actual.TrailingHeaders.Should()
                    .BeEquivalentTo(
                        JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, IEnumerable<string>>>>(
                            expected.TrailingHeaders));

                var content = await actual.Content.ReadAsStringAsync();
                content.Should().Be(EXPECTED_CONTENT);
            });
        }

        [Test]
        public void Should_MapToHttpResponseMessageFromRequest()
        {
            // Arrange
            var response = new HttpResponseMessage();
            response.Headers.Add("key", "value");

            const string EXPECTED_CONTENT = "content";

            var expected = new PaperEndpoint
            {
                Endpoint = new Uri("http://example.com"),
                Method = HttpMethod.Get,
                StatusCode = HttpStatusCode.Accepted,
                ReasonPhase = "OK",
                Headers = JsonConvert.SerializeObject(response.Headers),
                Content = EXPECTED_CONTENT
            };

            // Act
            var actual = expected.ToHttpResponseMessage(response.Headers);

            // Assert
            Assert.Multiple(async () =>
            {
                actual.RequestMessage?.RequestUri.Should().Be(expected.Endpoint);
                actual.RequestMessage?.Method.Should().Be(expected.Method);
                actual.StatusCode.Should().Be(expected.StatusCode);
                actual.ReasonPhrase.Should().Be(expected.ReasonPhase);
                actual.Headers.Should()
                    .BeEquivalentTo(
                        JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, IEnumerable<string>>>>(
                            expected.Headers));
                actual.TrailingHeaders.Should().BeEmpty();

                var content = await actual.Content.ReadAsStringAsync();
                content.Should().Be(EXPECTED_CONTENT);
            });
        }

        [Test]
        public void Should_MapToHttpResponseMessageFromMinimumData()
        {
            // Arrange
            var expected = new PaperEndpoint
            {
                Endpoint = new Uri("http://example.com"),
                Method = HttpMethod.Get,
                StatusCode = HttpStatusCode.Accepted
            };

            // Act
            var actual = expected.ToHttpResponseMessage(null);

            // Assert
            Assert.Multiple(async () =>
            {
                actual.RequestMessage?.RequestUri.Should().Be(expected.Endpoint);
                actual.RequestMessage?.Method.Should().Be(expected.Method);
                actual.StatusCode.Should().Be(expected.StatusCode);
                actual.ReasonPhrase.Should().Be("Accepted");
                actual.Headers.Should().BeEmpty();
                actual.TrailingHeaders.Should().BeEmpty();

                var content = await actual.Content.ReadAsStringAsync();
                content.Should().Be(string.Empty);
            });
        }
    }
}