using System;
using System.Collections.Generic;

namespace NewWordsBot
{
    public class Word
    {
        public string TheWord { get; } //TODO: rename
        public string Definition { get; } 
        public PartOfSpeech Form { get; }
        public LearningStage Stage { get; }
        public DateTime NextRepetition { get; }
        public DateTime AddedToDictionary { get; }

        public Word(string theWord, string definition, PartOfSpeech form, LearningStage stage, DateTime nextRepetition, DateTime addedToDictionary)
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