namespace NewWordsBot
{
    internal interface ILearningMethodology
    {
        Word OnRightResponse(Word word);
        Word OnWrongResponse(Word word);
    }
}