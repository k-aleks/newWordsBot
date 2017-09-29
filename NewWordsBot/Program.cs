using System.Collections.Generic;
using System.Threading;
using Telegram.Bot;

namespace NewWordsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var telegramBotClient = new TelegramBotClient(Config.Token);
            
            var usersStorageLocal = new UsersStorageLocal();
            var wordsStorageLocal = new WordsStorageLocal();
            var wordsDictionaryLocal = new WordsDictionaryLocal();
            var learningMethodology = new LearningMethodology(new TimeProvider());
            
            List<IHandler> handlers = new List<IHandler>()
            {
                new NewWordsHandler(usersStorageLocal, wordsStorageLocal, wordsDictionaryLocal, new NewWordsHandlerMessanger(telegramBotClient), learningMethodology),
                new BackgroundQuizHandler(usersStorageLocal, wordsStorageLocal, wordsDictionaryLocal, new PendingQuizRequests(),  new RandomWordsSelector(), learningMethodology, new BackgroundQuizHandlerMessanger(telegramBotClient)),
                new HelpHandler(telegramBotClient)
            };
            
            var bot = new Bot(telegramBotClient, handlers);
            bot.Start();
            Thread.Sleep(-1);
        }
    }
}