using System;
using System.Collections.Generic;
using NLog;

namespace NewWordsBot
{
    //TODO: delete this useless proxy
    public interface IWordsStorage
    {
        void AddOrUpdate(User user, Word word);
        Word GetNextReadyToRepeat(User user);
    }

    class WordsStorage : IWordsStorage
    {
        private readonly IStorageClient storageClient;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public WordsStorage(IStorageClient storageClient)
        {
            this.storageClient = storageClient;
        }

        public void AddOrUpdate(User user, Word word)
        {
            storageClient.AddOrUpdateWord(user, word);
        }

        public Word GetNextReadyToRepeat(User user)
        {
            return storageClient.FindWordWithNextRepetitionLessThenNow(user);
        }
    }
}