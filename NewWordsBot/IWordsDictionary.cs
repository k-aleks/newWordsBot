using System.Collections.Generic;

namespace NewWordsBot
{
    internal interface IWordsDictionary
    {
        List<string> FindDefinitions(string word);
    }

    class WordsDictionaryLocal : IWordsDictionary
    {
        public List<string> FindDefinitions(string word)
        {
            return new List<string>()
            {
                "something such as money or property that a person or company owns",
                "an item of text or media that has been put into a digital form that includes the right to use it",
                "a major benefit"
            };
        }
    }
}