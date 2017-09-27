using System.Collections.Generic;

namespace NewWordsBot
{
    internal interface IWordsStorage
    {
        void AddNewWord(User user, string word, string definition);
    }

    class WordsStorageLocal : IWordsStorage
    {
        public void AddNewWord(User user, string word, string definition)
        {
        }
    }
}