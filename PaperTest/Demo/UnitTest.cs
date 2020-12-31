using System;
using System.Threading;
using System.Threading.Tasks;
using ApiClient.Clients;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PaperTest;

namespace Demo
{
    public class UnitTest : Startup
    {
        protected override void SetupHttpClient(ServiceCollection serviceCollection)
        {
            var uri = new Uri("https://baconipsum.com/api/");
            serviceCollection.AddHttpClient("BaconClient", x => x.BaseAddress = uri)
                .AddHttpMessageHandler<PaperEndpointHandler>();
        }

        [Test]
        public async Task Test1()
        {
            // Arrange
            var client = new BaconIpsumApiClient(HttpClientFactory);

            // Act
            var actual = await client.GetAsync("", CancellationToken.None);

            // Assert
            Assert.Pass();
        }
    }
}