using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace NewWordsBot
{
    internal interface IUsersStorage
    {
        User GetOrRegisterUser(Chat chat);
        List<User> GetAllUsers();
    }

    class UsersStorageLocal : IUsersStorage
    {
        public User GetOrRegisterUser(Chat chat)
        {
            return new User(chat.Username, chat.Id);
        }

        public List<User> GetAllUsers()
        {
            throw new NotImplementedException();
        }
    }
}