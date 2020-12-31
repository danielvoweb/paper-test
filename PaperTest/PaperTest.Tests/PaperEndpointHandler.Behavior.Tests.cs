using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace PaperTest.Tests
{
    public class PaperEndpointHandlerBehaviorTests
    {
        private ICacheResolver _cacheResolver;
        private ILogger<PaperEndpointHandler> _logger;
        private HttpClient _sut;

        [SetUp]
        public void SetUp()
        {
            _cacheResolver = Substitute.For<ICacheResolver>();
            var repository = Substitute.For<IRepository<PaperEndpoint>>();
            var settings = Substitute.For<ISettings>();
            _logger = Substitute.For<ILogger<PaperEndpointHandler>>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_cacheResolver);
            serviceCollection.AddSingleton(repository);
            serviceCollection.AddSingleton(settings);
            serviceCollection.AddSingleton(_logger);
            serviceCollection.AddTransient<PaperEndpointHandler>();
            serviceCollection.AddHttpClient("PaperClient")
                .AddHttpMessageHandler<PaperEndpointHandler>();
            var provider = serviceCollection.BuildServiceProvider();
            var httpClientFactory = provider.GetService<IHttpClientFactory>();
            if (httpClientFactory == null)
                Assert.Fail($"Could not get {nameof(httpClientFactory)}");

            _sut = httpClientFactory.CreateClient("PaperClient");
        }

        [Test]
        public async Task Should_ReturnCachedResponseAndLogIfValidCache()
        {
            // Arrange
            (bool cached, HttpResponseMessage response) expected = (true, new HttpResponseMessage());

            _cacheResolver.BeforeRequest(Arg.Any<HttpRequestMessage>())
                .Returns(Task.FromResult(expected));

            // Act
            var actual = await _sut.GetAsync("http://example.com/api/");

            // Assert
            Assert.Multiple(() =>
            {
                actual.Should().Be(expected.response);
                _logger.Received().LogInformation(
                    $"Found cached HTTP response for {expected.response?.RequestMessage?.Method} {expected.response?.RequestMessage?.RequestUri}");
            });
        }

        [Test]
        public async Task Should_ReturnStoredResponseAndLogIfInvalidCache()
        {
            // Arrange
            (bool cached, HttpResponseMessage response) uncached = (false, new HttpResponseMessage());
            var expected = new HttpResponseMessage();

            _cacheResolver.BeforeRequest(Arg.Any<HttpRequestMessage>())
                .Returns(Task.FromResult(uncached));
            _cacheResolver.AfterRequest(Arg.Any<HttpResponseMessage>()).Returns(Task.FromResult(expected));

            // Act
            var actual = await _sut.GetAsync("http://example.com/api/");

            // Assert
            Assert.Multiple(() =>
            {
                actual.Should().Be(expected);
                _logger.Received().LogInformation(
                    $"Stored cached HTTP response for {expected.RequestMessage?.Method} {expected.RequestMessage?.RequestUri}");
            });
        }
    }
}