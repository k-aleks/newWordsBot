using System.Collections.Generic;
using NLog.Web.LayoutRenderers;
using Telegram.Bot.Types;

namespace NewWordsBot
{
    public interface IUsersStorage
    {
        User GetOrRegisterUser(Chat chat); //TODO: don't like Chat as a parameter
        List<User> GetAllUsers();
    }
}