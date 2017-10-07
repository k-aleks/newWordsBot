using System;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Driver;
using Telegram.Bot;

namespace NewWordsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var telegramBotClient = new TelegramBotClient(Config.TelegramToken);

            var storageClient = new StorageClient(new MongoClient(Config.MongoDbConnectionString), Config.DatabaseName, Config.UsersCollection, Config.WordsForUserCollectionPrefix);
            
            var usersStorage = new UsersStorage(storageClient, TimeSpan.FromMinutes(1));
            var wordsStorageLocal = new WordsStorage();
            var wordsDictionary = new WordsDictionary(new MacmillanApiClient(Config.MacmillanApiBaseUrl, Config.MacmillanApiKey));
            var learningMethodology = new LearningMethodology(new TimeProvider());
            
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