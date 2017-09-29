using System;
using System.Collections.Generic;

namespace NewWordsBot
{
    internal class Word
    {
        public string TheWord { get; } //TODO: rename
        public string Definition { get; } 
        public WordForm Form { get; }
        public LearningStage Stage { get; }
        public DateTime NextRepetition { get; }
        public DateTime AddedToDictionary { get; }

        public Word(string theWord, string definition, WordForm form, LearningStage stage, DateTime nextRepetition, DateTime addedToDictionary)
        {
            TheWord = theWord;
            Definition = definition;
            Form = form;
            Stage = stage;
            NextRepetition = nextRepetition;
            AddedToDictionary = addedToDictionary;
        }
    }
}