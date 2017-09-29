using System;
using System.Collections.Generic;

namespace NewWordsBot
{
    internal interface IWordsStorage
    {
        void AddOrUpdate(User user, Word word);
        Word GetNextReadyToRepeat(User user);
    }

    internal class Word
    {
        public string TheWord { get; } //TODO: rename
        public string Definition { get; }
        public WordForm Form { get; }
        public LearningStage Stage { get; }
        public DateTime NextRepetition { get; }
        public DateTime AddedToDictionary { get; }

        public Word(string word, string definition, WordForm wordForm, LearningStage learningStage, DateTime nextRepetition, DateTime addedToDictionary)
        {
            this.TheWord = word;
            this.Definition = definition;
            this.Form = wordForm;
            this.Stage = learningStage;
            this.NextRepetition = nextRepetition;
            this.AddedToDictionary = addedToDictionary;
        }
    }

    internal enum LearningStage
    {
        First_1m,
        Second_30m,
        Third_42h,
        Forth_14d,
        Fifth_60d
    }

    //TODO: complete
    enum WordForm
    {
        Noun,
        Verb,
        Adjective,
        PhrasalVerb
    }

    class WordsStorageLocal : IWordsStorage
    {
        public void AddOrUpdate(User user, Word word)
        {
        }

        public Word GetNextReadyToRepeat(User user)
        {
            throw new NotImplementedException();
        }
    }
}