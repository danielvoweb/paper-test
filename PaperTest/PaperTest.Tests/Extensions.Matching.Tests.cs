using System;
using System.Net.Http;
using FluentAssertions;
using NUnit.Framework;

namespace PaperTest.Tests
{
    public class ExtensionsMatchingTests
    {
        [Test]
        public void Should_MatchOnRequestUriAndMethod()
        {
            // Arrange
            var expectedUri = new Uri("http://example.com");

            var request = new HttpRequestMessage
            {
                RequestUri = expectedUri,
                Method = HttpMethod.Get
            };

            var endpoint = new PaperEndpoint
            {
                Endpoint = expectedUri,
                Method = HttpMethod.Get
            };

            // Act
            var actual = request.ThatMatches().Invoke(endpoint);

            // Assert
            actual.Should().BeTrue();
        }

        [Test]
        public void Should_NotMatchIfMethodDoesNotMatch()
        {
            // Arrange
            var expectedUri = new Uri("http://example.com");
            var expectedMethod = HttpMethod.Get;

            var request = new HttpRequestMessage
            {
                RequestUri = expectedUri,
                Method = expectedMethod
            };

            var endpoint = new PaperEndpoint
            {
                Endpoint = expectedUri,
                Method = HttpMethod.Delete
            };

            // Act
            var actual = request.ThatMatches().Invoke(endpoint);

            // Assert
            actual.Should().BeFalse();
        }

        [Test]
        public void Should_NotMatchIfUriDoesNotMatch()
        {
            // Arrange
            var expectedUri = new Uri("http://example.com");
            var expectedMethod = HttpMethod.Get;

            var request = new HttpRequestMessage
            {
                RequestUri = expectedUri,
                Method = expectedMethod
            };

            var endpoint = new PaperEndpoint
            {
                Endpoint = new Uri("http://anotherexample.com"),
                Method = expectedMethod
            };

            // Act
            var actual = request.ThatMatches().Invoke(endpoint);

            // Assert
            actual.Should().BeFalse();
        }

        [Test]
        public void Should_NotMatchIfRequestIsNull()
        {
            // Arrange
            var endpoint = new PaperEndpoint
            {
                Endpoint = new Uri("http://example.com"),
                Method = HttpMethod.Get
            };

            // Act
            var actual = ((HttpRequestMessage) null).ThatMatches().Invoke(endpoint);

            // Assert
            actual.Should().BeFalse();
        }
    }
}