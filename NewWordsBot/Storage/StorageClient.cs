using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;

namespace NewWordsBot
{
    public class StorageClient : IStorageClient
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly MongoClient mongoClient;
        private readonly string databaseName;
        private readonly string usersCollectionName;

        public StorageClient(MongoClient mongoClient, string databaseName, string usersCollectionName)
        {
            this.mongoClient = mongoClient;
            this.databaseName = databaseName;
            this.usersCollectionName = usersCollectionName;
        }

        public List<User> GetUsers()
        {
            var database = mongoClient.GetDatabase(databaseName);
            var collection = database.GetCollection<User>(usersCollectionName);
            var list = collection.Find(new BsonDocument()).ToList();
            logger.Debug($"Found {list.Count} users in database");
            return list;
        }

        public void InsertUser(User user)
        {
            var database = mongoClient.GetDatabase(databaseName);
            var collection = database.GetCollection<User>(usersCollectionName);
            collection.InsertOne(user);
            logger.Info($"Inserted new user into database: {user}");
        }
    }
}