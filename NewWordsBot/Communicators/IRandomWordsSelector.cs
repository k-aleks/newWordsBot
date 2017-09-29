namespace NewWordsBot
{
    internal interface IRandomWordsSelector
    {
        string Select(WordForm wordForm);
    }
}