using System.Collections.Generic;

namespace NewWordsBot
{
    public interface INewWordsHandlerMessanger
    {
        void SendNewWordConfirmation(User user, Word word);
        void SendError(User user);
        void SendSelectDefinitionRequest(User user, string word, List<string> definitions);
    }
}