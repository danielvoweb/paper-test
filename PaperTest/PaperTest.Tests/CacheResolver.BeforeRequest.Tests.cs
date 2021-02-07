using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace PaperTest.Tests
{
    public class CacheResolverBeforeRequestTests
    {
        [Test]
        public async Task Should_ReturnResponseFromExistingEndpoint()
        {
            // Arrange
            var expectedEndpoint = new PaperEndpoint
            {
                Method = HttpMethod.Get,
                StatusCode = HttpStatusCode.OK,
                TimeToLive = DateTime.Now.AddSeconds(30)
            };

            var repository = Substitute.For<IRepository<PaperEndpoint>>();
            var settings = Substitute.For<ISettings>();
            var request = Substitute.For<HttpRequestMessage>();

            repository.Get(Arg.Any
                <Func<PaperEndpoint, bool>>()).Returns(expectedEndpoint);

            var cacheResolver = new CacheResolver(repository, settings);

            // Act
            var actual = await cacheResolver.BeforeRequest(request);

            // Assert
            Assert.Multiple(async () =>
            {
                await repository.DidNotReceive().Remove(
                    Arg.Any<Expression<Func<PaperEndpoint, bool>>>());
                actual.cached.Should().BeTrue();
                actual.response.Should().BeEquivalentTo(
                    expectedEndpoint.ToHttpResponseMessage(request.Headers));
            });
        }

        [Test]
        public async Task Should_ReturnFalseIfNotCached()
        {
            // Arrange
            var repository = Substitute.For<IRepository<PaperEndpoint>>();
            repository.Get(Arg.Any<Func<PaperEndpoint, bool>>())
                .Returns(Task.FromResult<PaperEndpoint>(null));

            var settings = Substitute.For<ISettings>();
            var request = Substitute.For<HttpRequestMessage>();
            var cacheResolver = new CacheResolver(repository, settings);

            // Act
            var actual = await cacheResolver.BeforeRequest(request);

            // Assert
            Assert.Multiple(async () =>
            {
                await repository.DidNotReceive().Remove(
                    Arg.Any<Expression<Func<PaperEndpoint, bool>>>());
                actual.cached.Should().BeFalse();
                actual.response.Should().BeNull();
            });
        }

        [Test]
        public async Task Should_ReturnFalseIfInvalidTtlAndRemoveStale()
        {
            // Arrange
            var expectedEndpoint = new PaperEndpoint
            {
                Method = HttpMethod.Get,
                TimeToLive = DateTime.Now.AddSeconds(-2)
            };

            var repository = Substitute.For<IRepository<PaperEndpoint>>();
            repository.Get(Arg.Any
                <Func<PaperEndpoint, bool>>()).Returns(expectedEndpoint);
            
            var settings = Substitute.For<ISettings>();
            var request = Substitute.For<HttpRequestMessage>();

            var cacheResolver = new CacheResolver(repository, settings);

            // Act
            var actual = await cacheResolver.BeforeRequest(request);

            // Assert
            Assert.Multiple(async () =>
            {
                await repository.Received(1).Remove(
                    Arg.Any<Expression<Func<PaperEndpoint, bool>>>());
                actual.cached.Should().BeFalse();
                actual.response.Should().BeEquivalentTo(
                    expectedEndpoint.ToHttpResponseMessage(request.Headers));
            });
        }
    }
}