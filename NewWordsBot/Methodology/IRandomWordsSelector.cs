namespace NewWordsBot
{
    internal interface IRandomWordsSelector
    {
        string Select(WordForm wordForm);
    }

    class RandomWordsSelector : IRandomWordsSelector
    {
        public string Select(WordForm wordForm)
        {
            return "foobar";
        }
    }
}