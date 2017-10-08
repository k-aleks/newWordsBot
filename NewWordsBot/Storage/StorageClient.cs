using System;
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
        private readonly string wordsForUserCollectionPrefix;

        public StorageClient(MongoClient mongoClient, string databaseName, string usersCollectionName, string wordsForUserCollectionPrefix)
        {
            this.mongoClient = mongoClient;
            this.databaseName = databaseName;
            this.usersCollectionName = usersCollectionName;
            this.wordsForUserCollectionPrefix = wordsForUserCollectionPrefix;
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

        public void AddOrUpdateWord(User user, Word word)
        {
            var database = mongoClient.GetDatabase(databaseName);
            var collection = database.GetCollection<Word>(GetWordsCollecdtionName(user));
            collection.ReplaceOne(w => w.TheWord == word.TheWord, word, new UpdateOptions { IsUpsert = true });
        }

        public Word FindWordWithNextRepetitionLessThenNow(User user)
        {
            var database = mongoClient.GetDatabase(databaseName);
            var collection = database.GetCollection<Word>(GetWordsCollecdtionName(user));
            var res = collection.Find(w => w.NextRepetition < DateTime.UtcNow);
            return res.FirstOrDefault();
        }

        private string GetWordsCollecdtionName(User user)
        {
            return wordsForUserCollectionPrefix + user.Username;
        }
    }
}