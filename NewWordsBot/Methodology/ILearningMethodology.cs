namespace NewWordsBot
{
    public interface ILearningMethodology
    {
        Word OnRightResponse(Word word);
        Word OnWrongResponse(Word word);
        Word CreateNewWord(string word, string definition, PartOfSpeech form);
    }
}