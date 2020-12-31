using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NUnit.Framework;

namespace PaperTest
{
    public abstract class Startup
    {
        protected IHttpClientFactory HttpClientFactory;

        [OneTimeSetUp]
        public void Setup()
        {
            var serviceProvider = Services();
            HttpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        }

        protected virtual void SetupHttpClient(ServiceCollection serviceCollection)
        {
        }

        private ServiceProvider Services()
        {
            var serviceCollection = new ServiceCollection();

            var databaseSettings = DatabaseSettings();
            serviceCollection.AddSingleton<ISettings>(databaseSettings);
            serviceCollection.AddLogging(configure => configure.AddPaperLogger());

            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            var collection = database.GetCollection<PaperEndpoint>(databaseSettings.CollectionName);

            var paperEndpointRepository = new Repository<PaperEndpoint>(collection);
            serviceCollection.AddSingleton<IRepository<PaperEndpoint>>(paperEndpointRepository);
            serviceCollection.AddSingleton<ICacheResolver, CacheResolver>();
            serviceCollection.AddTransient<PaperEndpointHandler>();

            SetupHttpClient(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private DatabaseSettings DatabaseSettings()
        {
            return Configuration()
                .GetSection("PaperDatabaseSettings")
                .Get<DatabaseSettings>();
        }

        private static IConfiguration Configuration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true);
            return builder.Build();
        }
    }
}