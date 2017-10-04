using System;
using System.Collections.Generic;

namespace NewWordsBot
{
    public interface IWordsDictionary
    {
        DictionaryItem Find(string searchQuery);
    }

    public class WordsDictionary : IWordsDictionary
    {
        private readonly MacmillanSearchResultParser searchResultParser = new MacmillanSearchResultParser();
        readonly MacmillanGetItemResultParser getItemResultParser = new MacmillanGetItemResultParser();
        private readonly MacmillanApiClient apiClient;

        public WordsDictionary(MacmillanApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public DictionaryItem Find(string searchQuery)
        {
            var searchResult = apiClient.Search(searchQuery);
            string itemId;
            if (!searchResultParser.TryParse(searchResult, out itemId))
                throw new Exception("TODO: organize fallback");
            var getItemResult = apiClient.GetItem(itemId);
            if (!getItemResultParser.TryParse(getItemResult, out var definitions, out var partofspeech))
                throw new Exception("TODO: organize fallback");
            return new DictionaryItem(searchQuery, definitions, partofspeech);
        }
    }
}