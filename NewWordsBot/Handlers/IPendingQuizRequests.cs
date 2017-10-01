using System;
using System.Collections.Generic;

namespace NewWordsBot
{
    public interface IPendingQuizRequests
    {
        bool ContainsRequest(User user);
        bool TryGet(User user, out Word word, out int rightVariantIndex);
        void Add(User user, Word word, int rightVariantIndex);
        void Remove(User user);
    }

    class PendingQuizRequests : IPendingQuizRequests
    {
        readonly Dictionary<User, Tuple<Word, int>> storage = new Dictionary<User, Tuple<Word, int>>();

        public bool ContainsRequest(User user)
        {
            return storage.ContainsKey(user);
        }

        public bool TryGet(User user, out Word word, out int rightVariantIndex)
        {
            word = null;
            rightVariantIndex = -1;
            Tuple<Word, int> value;
            if (storage.TryGetValue(user, out value))
            {
                word = value.Item1;
                rightVariantIndex = value.Item2;
                return true;
            }
            return false;
        }

        public void Add(User user, Word word, int rightVariantIndex)
        {
            storage.Add(user, new Tuple<Word, int>(word, rightVariantIndex));
        }

        public void Remove(User user)
        {
            storage.Remove(user);
        }
    }
}