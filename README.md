# Paper Test
_The C# integration testing library for self-initializing fakes with MongoDB._

Paper Test is a testing library that helps you quickly setup self-initializing fakes when integration testing a new API. Paper Test is designed to help you find balance in your integration testing by shortening latency, improving stability, and test speed so automation can be an option.

## What is a Paper Test?

### pa·per
*/ˈpāpər/*

*noun*
1. material manufactured in thin sheets from the pulp of wood or other fibrous substances, used for writing, drawing, or printing on, or as **wrapping material**.
   "a sheet of paper"
2. denoting something that is officially documented but has **no real existence**. "a paper profit"

A paper test is an integration test that draws from Martin Fowler's concepts on [Self-Initializing Fakes](https://martinfowler.com/bliki/SelfInitializingFake.html). Essentially, after the initial request made to a remote service, the response is stored and used for further testing. This approach improves a test's stability since your subsequent tests are not made over network or to a volatile data source, but rather a local store.

Paper Test draws its name from the definition of "paper" being a "wrapping material" or "something that is officially documented but has no real existence", i.e. the stored response used as a test Fake.

## Problem

Self-Initializing Fakes is a testing concept that has been around for a while, but most solutions are specific and homegrown...or a Postman collection of cURL scripts. In the world of REST API-everything, the Paper Test library offers a tool for the C# developer to capture the behavior of a RESTful API into an executable specification, i.e. automated tests.

### Automated test vs. Postman

While Postman and other API testing tools are great for testing out an API and test how it works, creating an automated test helps you capture that behavior and continue to test against it in the future. Writing Paper Tests that can be incorporated into your build pipeline can be a powerful tool to inform you when an API's behavior has changed.

## How to use Paper Test

To start using Paper Test, install the latest version of Paper Test from NuGet into your test project.

Add an `appsettings.json` if you don't have one already with connection strings to a MongoDB instance. For more details on setting up a simple Docker instance of MongoDb as used in this example, see: [link].

Once you've configured this section in the `appsetting.json` of your test project, please make sure you set the file to **Copy Always** on build to your output directory. Without a copy in your output directory, your tests will not be able to connect to your MongoDb instance where your fakes live.

```json
{
    "PaperDatabaseSettings": {
        "CollectionName": "PaperEndpoints",
        "ConnectionString": "mongodb://localhost:27017",
        "DatabaseName": "PaperEndpointsDb",
        "SecondsToLive": 1
    },
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        }
    }
}
```
Once you have a MongoDb instance available and connected to your test project, you can start testing with a few easy steps! First, reference the Startup base class in the Paper Test project and override the `SetupHttpClient` method. The Paper Test library handles dependency registry and MongoDB setup for you, so you can get to work.

Within the `SetupHttpClient` method, you can easily extend any delegating handlers you or other dependencies that need to be setup for your client-under-test. Be sure to add the `PaperEndpointHandler` to record your requests and responses.

```c#
// Setup a test class with your favorite framework and reference
// the base Startup class from the Paper Test library
public class BaconIpsumTests : Startup
{
    private BaconIpsumApiClient _client;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    // Setup your HttpClient access to the API you wish to to test
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
    public async Task Should_DoSomething()
    {
        // Arrange
        var requestObject = new BaconIpsumRequestObject(BaconIpsumFillerType.MeatAndFiller);
        
        // Act
        var actual = await _client.GetAllAsync(requestObject, _cancellationToken);
        
        // Assert
        // actual...Does something...
    }
}
```

For full code examples, visit: https://github.com/danielvoweb/paper-test

<!--TODO Document caching -->
<!--TODO Document mongodb setup with docker -->
<!--TODO Document settings -->
<!--TODO Document how it works -->
<!--TODO Test Cache times -->
<!--TODO Write article on Self-Initializing Fakes -->
<!--TODO Benefits of closing the round-trip, errors, size, timeouts,Test Framework Agnostic, etc -->
<!--TODO Write follow-up article on Integration Testing Strategies -->
<!--TODO Document sticking points, number of records created, etc -->