using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using log4net;
using log4net.Config;
using MongoDB.Driver;
using Telegram.Bot;

namespace NewWordsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggingRepo = LogManager.GetRepository(typeof(Program).Assembly);
            XmlConfigurator.ConfigureAndWatch(loggingRepo, new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.config.xml")));
            
            var telegramBotClient = new TelegramBotClient(Config.TelegramToken);

            var storageClient = new StorageClient(new MongoClient(Config.MongoDbConnectionString), Config.DatabaseName, Config.UsersCollection, Config.WordsForUserCollectionPrefix);
            
            var usersStorage = new UsersStorage(storageClient, TimeSpan.FromHours(1) /*Don't expect to add new users in concurrent right now*/);
            var wordsStorageLocal = new WordsStorage(storageClient);
            var wordsDictionary = new WordsDictionary(new MacmillanApiClient(Config.MacmillanApiBaseUrl, Config.MacmillanApiKey));
            var timeProvider = Config.TestMode ? (ITimeProvider) new TimeProviderForTests() : new TimeProvider();
            var learningMethodology = new LearningMethodology(timeProvider);
            
            List<IHandler> handlers = new List<IHandler>()
            {
                new NewWordsHandler(usersStorage, wordsStorageLocal, wordsDictionary, new NewWordsHandlerMessanger(telegramBotClient), learningMethodology),
                new BackgroundQuizHandler(usersStorage, wordsStorageLocal, wordsDictionary, new PendingQuizRequests(),  new RandomWordDefinitionSelector(wordsDictionary), learningMethodology, new BackgroundQuizHandlerMessanger(telegramBotClient)),
                new HelpHandler(telegramBotClient)
            };
            
            var bot = new Bot(telegramBotClient, handlers);
            bot.Start();
            Thread.Sleep(-1);
        }
    }
}