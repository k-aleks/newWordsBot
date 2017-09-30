namespace NewWordsBot
{
    internal interface ILearningMethodology
    {
        Word OnRightResponse(Word word);
        Word OnWrongResponse(Word word);
        Word CreateNewWord(string word, string definition, WordForm form);
    }
}