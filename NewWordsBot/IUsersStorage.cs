using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace NewWordsBot
{
    public interface IUsersStorage
    {
        User GetOrRegisterUser(Chat chat);
        List<User> GetAllUsers();
    }

    class UsersStorageLocal : IUsersStorage
    {
        Dictionary<string, User> users = new Dictionary<string, User>();
        
        public User GetOrRegisterUser(Chat chat)
        {
            if (users.ContainsKey(chat.Username))
                return users[chat.Username];
            
            var user =  new User(chat.Username, chat.Id);
            users[user.Username] = user;
            return user;
        }

        public List<User> GetAllUsers()
        {
            return users.Values.ToList();
        }
    }
}