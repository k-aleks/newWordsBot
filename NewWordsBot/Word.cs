using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NewWordsBot
{
    public class Word
    {
        [BsonId]
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

        protected bool Equals(Word other)
        {
            return string.Equals(TheWord, other.TheWord)
                   && string.Equals(Definition, other.Definition)
                   && Form == other.Form
                   && Stage == other.Stage
                   && Math.Abs(NextRepetition.Ticks - other.NextRepetition.Ticks) <= 10000
                   && Math.Abs(AddedToDictionary.Ticks - other.AddedToDictionary.Ticks) <= 10000;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Word) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (TheWord != null ? TheWord.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Definition != null ? Definition.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Form;
                hashCode = (hashCode * 397) ^ (int) Stage;
                hashCode = (hashCode * 397) ^ NextRepetition.GetHashCode();
                hashCode = (hashCode * 397) ^ AddedToDictionary.GetHashCode();
                return hashCode;
            }
        }
    }
}