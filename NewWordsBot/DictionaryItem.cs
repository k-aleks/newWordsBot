using System.Collections.Generic;

namespace NewWordsBot
{
    public class DictionaryItem
    {
        public string Word { get; } 
        public List<string> Definitions { get; }
        public WordForm Form { get; }

        public DictionaryItem(string word, List<string> definitions, WordForm form)
        {
            Word = word;
            Definitions = definitions;
            Form = form;
        }
    }
}