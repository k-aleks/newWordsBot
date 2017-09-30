using System.Collections.Generic;

namespace NewWordsBot
{
    public interface IWordsDictionary
    {
        DictionaryItem Find(string word);
    }

    class WordsDictionaryLocal : IWordsDictionary
    {
        public DictionaryItem Find(string word)
        {
            var definitions = new List<string>()
            {
                "something such as money or property that a person or company owns",
                "an item of text or media that has been put into a digital form that includes the right to use it",
                "a major benefit"
            };
            return new DictionaryItem(word, definitions, WordForm.Noun);
        }
    }
}