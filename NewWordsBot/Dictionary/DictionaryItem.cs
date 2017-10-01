using System.Collections.Generic;

namespace NewWordsBot
{
    public class DictionaryItem
    {
        public string Word { get; } 
        public List<string> Definitions { get; }
        public PartOfSpeech Form { get; }

        public DictionaryItem(string word, List<string> definitions, PartOfSpeech form)
        {
            Word = word;
            Definitions = definitions;
            Form = form;
        }
    }
}