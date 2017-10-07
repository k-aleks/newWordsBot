using System.Collections.Generic;

namespace NewWordsBot
{
    public interface IStorageClient
    {
        List<User> GetUsers();
        void InsertUser(User user);
        void AddOrUpdateWord(User user, Word word);
        Word FindWordWithNextRepetitionLessThenNow(User user);
    }
}