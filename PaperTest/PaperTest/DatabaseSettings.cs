namespace PaperTest
{
    public interface ISettings
    {
        public int SecondsToLive { get; set; }
    }
    
    public class DatabaseSettings : ISettings
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public int SecondsToLive { get; set; } = 84600;
    }
}