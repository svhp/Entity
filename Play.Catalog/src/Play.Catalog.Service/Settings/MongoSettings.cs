namespace Play.Catalog.Service.Settings{
    public class MongoSettings{
        public string Host { get; init; }
        public int Port { get; init; }
        public string ConnectionString{
            get{
                return $"mongodb://{Host}:{Port}";
            }
        }
    }
}