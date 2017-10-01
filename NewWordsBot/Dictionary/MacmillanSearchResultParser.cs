using Newtonsoft.Json;

namespace NewWordsBot
{
    public class MacmillanSearchResultParser
    {
        public bool TryParse(string jsonResult, out string itemId)
        {
            itemId = null;
            var res = JsonConvert.DeserializeObject<SearchResult>(jsonResult);
            if (res.ResultNumber <= 0)
                return false;
            itemId = res.Results[0].EntryId;
            return true;
        }
    }

    public class SearchResult
    {
        public int ResultNumber { get; set; }
        public ResultItem[] Results { get; set; }
        public string DictionaryCode { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageNumber { get; set; }
    }

    public class ResultItem
    {
        public string EntryLabel { get; set; }
        public string EntryUrl { get; set; }
        public string EntryId { get; set; }
    }
}