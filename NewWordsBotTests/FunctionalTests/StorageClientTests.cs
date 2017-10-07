using System;
using FluentAssertions;
using MongoDB.Driver;
using NewWordsBot;
using Xunit;

namespace NewWordsBotTests.FunctionalTests
{
    public class StorageClientTests
    {
        [Fact]
        public void GetUsers_should_return_all_users_after_InsertUser_call()
        {
            var u1 = new User(Guid.NewGuid(), "user1", 123, DateTime.UtcNow);
            var u2 = new User(Guid.NewGuid(), "user2", 123, DateTime.UtcNow);
            var u3 = new User(Guid.NewGuid(), "user3", 123, DateTime.UtcNow);
            
            var mongoClient = new MongoClient(Config.MongoDbConnectionString);
            
            if (!Config.DatabaseName.EndsWith("-test"))
                throw new Exception("A-a-a-a, don't clear working database");
            mongoClient
                .GetDatabase(Config.DatabaseName)
                .GetCollection<User>(Config.UsersCollection)
                .DeleteMany(u => true);
            
            var storageClient = new StorageClient(mongoClient, Config.DatabaseName, Config.UsersCollection);
            
            storageClient.InsertUser(u1);
            var users = storageClient.GetUsers();
            users.Should().Equal(u1);
            
            storageClient.InsertUser(u2);
            storageClient.GetUsers().Should().Equal(u1, u2);
            
            storageClient.InsertUser(u3);
            storageClient.GetUsers().Should().Equal(u1, u2, u3);
        }
        
    }
}