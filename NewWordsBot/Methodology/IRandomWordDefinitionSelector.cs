using System;
using System.IO;
using log4net;

namespace NewWordsBot
{
    public interface IRandomWordDefinitionSelector
    {
        string Select(PartOfSpeech partOfSpeech);
    }

    class RandomWordDefinitionSelector : IRandomWordDefinitionSelector
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(RandomWordDefinitionSelector));
        private readonly Random rnd = new Random();
        private readonly IWordsDictionary dictionary;
        private readonly string[] words;
        private int maxAttempts = 3;

        public RandomWordDefinitionSelector(IWordsDictionary dictionary)
        {
            this.dictionary = dictionary;
            words = File.ReadAllLines("Resources/words");
            logger.Info($"Initialized words for randoms difinitions. Words count: {words.Length}");
        }

        public string Select(PartOfSpeech partOfSpeech)
        {
            DictionaryItem dictionaryItem;
            for (int i = 0; i < maxAttempts; i++)
            {
                dictionaryItem = GetRandomWordFromDictionary();
                if (partOfSpeech == dictionaryItem.PartOfSpeech)
                {
                    if (dictionaryItem.Definitions.Count > 0)
                    {
                        return GetRandomDefinition(dictionaryItem);
                    }
                }
            }
            logger.Warn($"Didn't manage to find '{partOfSpeech}' in {maxAttempts} attempts. Let's return any word");
            while ((dictionaryItem = GetRandomWordFromDictionary()).Definitions.Count == 0)
            {
            }
            return GetRandomDefinition(dictionaryItem);
        }

        private string GetRandomDefinition(DictionaryItem dictionaryItem)
        {
            return dictionaryItem.Definitions[rnd.Next(0, dictionaryItem.Definitions.Count)];
        }

        private DictionaryItem GetRandomWordFromDictionary()
        {
            var rndWord = words[rnd.Next(0, words.Length)];
            var dictionaryItem = dictionary.Find(rndWord);
            return dictionaryItem;
        }
    }
}