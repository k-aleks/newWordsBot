using Telegram.Bot.Types;

namespace NewWordsBot
{
    interface ICommunicator
    {
        bool TryHandleRequest(Message message);
        bool TryHandleCallback(CallbackQuery callback);
    }
}