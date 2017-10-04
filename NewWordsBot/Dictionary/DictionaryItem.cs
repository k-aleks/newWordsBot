using System.Collections.Generic;

namespace NewWordsBot
{
    public class DictionaryItem
    {
        public string Word { get; } 
        public List<string> Definitions { get; }
        public PartOfSpeech PartOfSpeech { get; }

        public DictionaryItem(string word, List<string> definitions, PartOfSpeech partOfSpeech)
        {
            Word = word;
            Definitions = definitions;
            PartOfSpeech = partOfSpeech;
        }
    }
}