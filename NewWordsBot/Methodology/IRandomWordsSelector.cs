namespace NewWordsBot
{
    public interface IRandomWordsSelector
    {
        string Select(PartOfSpeech partOfSpeech);
    }

    class RandomWordsSelector : IRandomWordsSelector
    {
        public string Select(PartOfSpeech partOfSpeech)
        {
            return "device";
        }
    }
}