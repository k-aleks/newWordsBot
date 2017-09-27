using System;

namespace NewWordsBot
{
    internal interface IUsersStorage
    {
        User GetOrRegisterUser(string username);
    }

    class UsersStorageLocal : IUsersStorage
    {
        public User GetOrRegisterUser(string username)
        {
            return new User(username);
        }
    }
}