using System;
using System.Collections.Generic;

namespace NewWordsBot
{
    internal interface IWordsStorage
    {
        void AddOrUpdate(User user, Word word);
        Word GetNextReadyToRepeat(User user);
    }

    class WordsStorageLocal : IWordsStorage
    {
        Dictionary<User, Dictionary<string, Word>> storage = new Dictionary<User, Dictionary<string, Word>>();
        
        public void AddOrUpdate(User user, Word word)
        {
            if (!storage.ContainsKey(user))
                storage.Add(user, new Dictionary<string, Word>());
            storage[user][word.TheWord] = word;
        }

        public Word GetNextReadyToRepeat(User user)
        {
            if (!storage.ContainsKey(user))
                return null;
            foreach (var kvp in storage[user])
            {
                if (kvp.Value.NextRepetition <= DateTime.UtcNow)
                    return kvp.Value;
            }
            return null;
        }
    }
}