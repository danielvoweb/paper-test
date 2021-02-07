using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiClient.Clients.BaconIpsum;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PaperTest;

namespace Demo
{
    public class BaconIpsumTests : Startup
    {
        private BaconIpsumApiClient _client;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        // ReSharper disable StringLiteralTypo
        private const string LOREM_IPSUM =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        private const string EXPECTED_START = "Bacon ipsum dolor amet"; // Note: Actual behavior differs from documentation
        // ReSharper enable StringLiteralTypo

        private readonly string[] _fillerList = Helpers.SentenceToStringArray(LOREM_IPSUM);
        
        // Since these variable values can change the endpoint, keeping these variables constant will help caching
        private const int PARAGRAPH_COUNT = 12;
        private const int SENTENCE_COUNT = 6;

        protected override void SetupHttpClient(ServiceCollection serviceCollection)
        {
            var apiUri = new Uri("https://baconipsum.com/api/");
            serviceCollection.AddHttpClient("BaconClient", x => x.BaseAddress = apiUri)
                .AddHttpMessageHandler<PaperEndpointHandler>();
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            _client = new BaconIpsumApiClient(HttpClientFactory);
        }

        [Test]
        public async Task Should_DefaultMeatAndFillerWithFiveParagraphs()
        {
            // Arrange
            var requestObject = new BaconIpsumRequestObject(BaconIpsumFillerType.MeatAndFiller);

            // Act
            var actual = await _client.GetAllAsync(requestObject, _cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                var paragraphs = actual.ToList();
                paragraphs.Should().HaveCount(5);
                var words = Helpers.SentenceToStringArray(string.Join(' ', paragraphs));
                words.Any(word => _fillerList.Contains(word))
                    .Should().BeTrue(because: "Response should contain filler");
            }
        }

        [Test]
        public async Task Should_DefaultAllMeatWithFiveParagraphs()
        {
            // Arrange
            var requestObject = new BaconIpsumRequestObject(BaconIpsumFillerType.AllMeat);

            // Act
            var actual = await _client.GetAllAsync(requestObject, _cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                var paragraphs = actual.ToList();
                paragraphs.Should().HaveCount(5);
                var words = Helpers.SentenceToStringArray(string.Join(' ', paragraphs));
                words.Any(word => _fillerList.Contains(word))
                    .Should().BeFalse(because: "Response should not contain filler");
            }
        }

        [Test]
        public async Task Should_SetParagraphCount()
        {
            // Arrange
            var requestObject = new BaconIpsumRequestObject(BaconIpsumFillerType.MeatAndFiller)
            {
                ParagraphCount = PARAGRAPH_COUNT
            };

            // Act
            var actual = await _client.GetAllAsync(requestObject, _cancellationToken);

            // Assert
            actual.Should().HaveCount(PARAGRAPH_COUNT);
        }

        [Test]
        public async Task Should_SetSentenceCount()
        {
            // Arrange
            var requestObject = new BaconIpsumRequestObject(BaconIpsumFillerType.MeatAndFiller)
            {
                SentenceCount = SENTENCE_COUNT
            };

            // Act
            var actual = await _client.GetAllAsync(requestObject, _cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                var paragraphs = actual.ToList();
                paragraphs.Should().HaveCount(1);
                var sentences = paragraphs.First().Split('.')
                    .Where(sentence => !string.IsNullOrWhiteSpace(sentence));
                sentences.Count().Should().Be(SENTENCE_COUNT);
            }
        }
        
        [Test]
        public async Task Should_SetSentenceCountAndOverrideParagraphCount()
        {
            // Arrange
            var requestObject = new BaconIpsumRequestObject(BaconIpsumFillerType.MeatAndFiller)
            {
                ParagraphCount = PARAGRAPH_COUNT,
                SentenceCount = SENTENCE_COUNT
            };

            // Act
            var actual = await _client.GetAllAsync(requestObject, _cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                var paragraphs = actual.ToList();
                paragraphs.Should().HaveCount(1);
                var sentences = paragraphs.First().Split('.')
                    .Where(sentence => !string.IsNullOrWhiteSpace(sentence));
                sentences.Count().Should().Be(SENTENCE_COUNT);
            }
        }
        
        [Test]
        public async Task Should_SetStartWithLorem()
        {
            // Arrange
            var requestObject = new BaconIpsumRequestObject(BaconIpsumFillerType.MeatAndFiller)
            {
                StartWithLorem = true
            };
            
            // Act
            var actual = await _client.GetAllAsync(requestObject, _cancellationToken);

            // Assert
            var firstParagraph = actual.First();
            firstParagraph.StartsWith(EXPECTED_START)
                .Should().BeTrue(because: $"Response should begin with '{EXPECTED_START}'");
        }
        
        [Test]
        public async Task Should_SupportMultipleVariables()
        {
            // Arrange
            var requestObject = new BaconIpsumRequestObject(BaconIpsumFillerType.MeatAndFiller)
            {
                ParagraphCount = PARAGRAPH_COUNT,
                SentenceCount = SENTENCE_COUNT,
                StartWithLorem = true
            };
            
            // Act
            var actual = await _client.GetAllAsync(requestObject, _cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                var paragraphs = actual.ToList();
                paragraphs.Should().HaveCount(1);
                var sentences = paragraphs.First().Split('.')
                    .Where(sentence => !string.IsNullOrWhiteSpace(sentence));
                sentences.Count().Should().Be(SENTENCE_COUNT);
                var firstParagraph = paragraphs.First();
                firstParagraph.StartsWith(EXPECTED_START)
                    .Should().BeTrue(because: $"Response should begin with '{EXPECTED_START}'");    
            }
        }
    }
}