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
            var wordsDictionary = new WordsDictionary(new MacmillanApiClient(Config.MacmillanApiBaseUrl, Config.MacmillanApiKey));
            var learningMethodology = new LearningMethodology(new TimeProvider());
            
            List<IHandler> handlers = new List<IHandler>()
            {
                new NewWordsHandler(usersStorageLocal, wordsStorageLocal, wordsDictionary, new NewWordsHandlerMessanger(telegramBotClient), learningMethodology),
                new BackgroundQuizHandler(usersStorageLocal, wordsStorageLocal, wordsDictionary, new PendingQuizRequests(),  new RandomWordDefinitionSelector(wordsDictionary), learningMethodology, new BackgroundQuizHandlerMessanger(telegramBotClient)),
                new HelpHandler(telegramBotClient)
            };
            
            var bot = new Bot(telegramBotClient, handlers);
            bot.Start();
            Thread.Sleep(-1);
        }
    }
}