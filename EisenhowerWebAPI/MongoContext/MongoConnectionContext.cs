using EisenhowerWebAPI.Models;
using MongoDB.Driver;

namespace EisenhowerWebAPI.MongoContext
{
    public class MongoConnectionContext
    {
        private readonly IMongoDatabase _database;

        public MongoConnectionContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDBConnection");
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("EisenhoverDB");
        }

        public IMongoCollection<UserModel> Users { get { return _database.GetCollection<UserModel>("Users"); } }
        public IMongoCollection<TasksPoolModel> TasksPool { get { return _database.GetCollection<TasksPoolModel>("TasksPool"); } }
    }
}
