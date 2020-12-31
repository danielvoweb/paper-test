using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace PaperTest.Tests
{
    public class CacheResolverAfterRequestTests
    {
        [Test]
        public async Task Should_ReturnResponseFromNewResponse()
        {
            // Arrange
            const int SECONDS_TO_LIVE = 30;

            var expectedEndpoint = new PaperEndpoint
            {
                Method = HttpMethod.Get
            };

            var repository = Substitute.For<IRepository<PaperEndpoint>>();
            var settings = Substitute.For<ISettings>();
            settings.SecondsToLive = SECONDS_TO_LIVE;
            
            var response = Substitute.For<HttpResponseMessage>();

            repository.Create(Arg.Any<PaperEndpoint>())
                .Returns(expectedEndpoint);

            var cacheResolver = new CacheResolver(repository, settings);

            // Act
            var actual = await cacheResolver.AfterRequest(response);

            // Assert
            actual.Should().BeEquivalentTo(expectedEndpoint.ToHttpResponseMessage(response.Headers));
        }
    }
}