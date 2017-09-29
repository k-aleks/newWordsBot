namespace NewWordsBot
{
    internal interface IPendingRequests
    {
        bool ContainsRequest(User user);
        bool TryGet(User user, out Word word, out int rightVariantIndex);
        void Add(User user, Word word, int rightVariantIndex);
        void Remove(User user);
    }
}