using System.Threading;
using Telegram.Bot;

namespace NewWordsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot(new TelegramBotClient(Config.Token), new UsersStorageLocal(), new WordsStorageLocal(), new WordsDictionaryLocal());
            bot.Start();
            Thread.Sleep(-1);
        }
    }
}