using log4net;

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
        private readonly ILog logger = LogManager.GetLogger(typeof(WordsStorage));

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