using System;
using System.Collections.Generic;
using log4net;

namespace NewWordsBot
{
    public interface IWordsDictionary
    {
        DictionaryItem Find(string searchQuery);
    }

    public class WordsDictionary : IWordsDictionary
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(NewWordsHandler));
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
            {
                logger.Error($"Error in parsing API search result. SearchResul: {searchResult}");
                return new DictionaryItem(searchQuery, new List<string>(), PartOfSpeech.Unknown); 
            }
            var getItemResult = apiClient.GetItem(itemId);
            if (!getItemResultParser.TryParse(getItemResult, out var definitions, out var partofspeech))
            {
                logger.Error($"Error in parsing API get item result. GetItemResult: {getItemResult}");
                return new DictionaryItem(searchQuery, new List<string>(), PartOfSpeech.Unknown); 
            }
            return new DictionaryItem(searchQuery, definitions, partofspeech);
        }
    }
}