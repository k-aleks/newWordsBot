using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using NewWordsBot;
using NSubstitute;
using NSubstitute.ClearExtensions;
using Telegram.Bot.Types;
using Xunit;
using User = NewWordsBot.User;

namespace NewWordsBotTests
{
    public class UsersStorageTests
    {
        [Fact]
        public void GetOrRegisterUser_should_save_user_to_storage_if_there_is_no_user_with_such_username()
        {
            var storageClient = Substitute.For<IStorageClient>();
            
            var usersStorage = new UsersStorage(storageClient, TimeSpan.FromMilliseconds(100));

            var res = usersStorage.GetOrRegisterUser(new Chat()
            {
                Username = "user1",
                Id = 123
            });

            res.Username.Should().Be("user1");
            res.ChatId.Should().Be(123);
            storageClient.Received().InsertUser(Arg.Any<User>());
        }
        
        [Fact]
        public void GetOrRegisterUser_should_not_save_user_to_storage_if_there_is_already_user_with_such_username()
        {
            var storageClient = Substitute.For<IStorageClient>();
            
            var usersStorage = new UsersStorage(storageClient, TimeSpan.FromMilliseconds(100));

            var res = usersStorage.GetOrRegisterUser(new Chat()
            {
                Username = "user1",
                Id = 123
            });
            
            storageClient.ClearReceivedCalls();
            
            res = usersStorage.GetOrRegisterUser(new Chat()
            {
                Username = "user1",
                Id = 123
            });

            res.Username.Should().Be("user1");
            res.ChatId.Should().Be(123);
            storageClient.DidNotReceive().InsertUser(Arg.Any<User>());
        }
        
        [Fact]
        public void InternalUpdateCacheRoutine_should_replace_user_if_new_RegisteredDate_is_newer()
        {
            var u1 = new User(Guid.NewGuid(), "user1", 123, DateTime.UtcNow);
            var u2 = new User(Guid.NewGuid(), "user1", 321, DateTime.UtcNow.AddSeconds(10));
            
            var storageClient = Substitute.For<IStorageClient>();
            storageClient.GetUsers().Returns(new List<User>() { u1 });
            
            var usersStorage = new UsersStorage(storageClient, TimeSpan.FromMilliseconds(100));
            
            Thread.Sleep(1000);
            var users = usersStorage.GetAllUsers();
            users.Should().Equal(u1);
            
            storageClient.GetUsers().Returns(new List<User>() { u2 });
            Thread.Sleep(500);
            users = usersStorage.GetAllUsers();
            users.Should().Equal(u2);
        }
        
        [Fact]
        public void InternalUpdateCacheRoutine_should_not_replace_user_if_new_RegisteredDate_is_older()
        {
            var u1 = new User(Guid.NewGuid(), "user1", 123, DateTime.UtcNow);
            var u2 = new User(Guid.NewGuid(), "user1", 321, DateTime.UtcNow.AddSeconds(-10));
            
            var storageClient = Substitute.For<IStorageClient>();
            storageClient.GetUsers().Returns(new List<User>() { u1 });
            
            var usersStorage = new UsersStorage(storageClient, TimeSpan.FromMilliseconds(100));
            
            Thread.Sleep(1000);
            var users = usersStorage.GetAllUsers();
            users.Should().Equal(u1);
            
            storageClient.GetUsers().Returns(new List<User>() { u2 });
            Thread.Sleep(500);
            users = usersStorage.GetAllUsers();
            users.Should().Equal(u1);
        }
    }
}