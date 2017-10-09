using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Telegram.Bot.Types;

namespace NewWordsBot
{
    public class UsersStorage : IUsersStorage
    {
        private readonly IStorageClient storageClient;
        private readonly TimeSpan cacheUpdatePeriod;
        private readonly ILog logger = LogManager.GetLogger(typeof(UsersStorage));
        private readonly Dictionary<string, User> usersCache = new Dictionary<string, User>();

        public UsersStorage(IStorageClient storageClient, TimeSpan cacheUpdatePeriod)
        {
            this.storageClient = storageClient;
            this.cacheUpdatePeriod = cacheUpdatePeriod;
            RunChacheUpdater();
        }

        public User GetOrRegisterUser(Chat chat)
        {
            lock(usersCache)
                if (usersCache.ContainsKey(chat.Username) &&
                    usersCache[chat.Username].Username == chat.Username &&
                    usersCache[chat.Username].ChatId == chat.Id)
                {
                    return usersCache[chat.Username];
                }

            var user = new User(Guid.NewGuid(), chat.Username, chat.Id, DateTime.UtcNow);
            storageClient.InsertUser(user);
            AddOrUpdateUserToCache(user);
            return user;
        }

        public List<User> GetAllUsers()
        {
            lock(usersCache)
                return usersCache.Values.ToList();
        }

        private void RunChacheUpdater()
        {
            Task.Run(() => InternalUpdateCacheRoutine());
        }

        private void InternalUpdateCacheRoutine()
        {
            while (true)
            {
                try
                {
                    var list = storageClient.GetUsers();
                    if (list != null)
                        foreach (var user in list)
                        {
                            AddOrUpdateUserToCache(user);
                        }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
                Thread.Sleep(cacheUpdatePeriod);
            }
        }

        private void AddOrUpdateUserToCache(User user)
        {
            lock (usersCache)
            {
                if (!usersCache.ContainsKey(user.Username))
                {
                    usersCache.Add(user.Username, user);
                }
                else
                {
                    if (usersCache[user.Username].RegisteredDate < user.RegisteredDate)
                        usersCache[user.Username] = user;
                }
            }
        }
    }
}