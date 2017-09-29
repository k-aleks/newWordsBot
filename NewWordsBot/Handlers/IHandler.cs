using Telegram.Bot.Types;

namespace NewWordsBot
{
    interface IHandler
    {
        void Start();
        bool TryHandleRequest(Message message);
        bool TryHandleCallback(CallbackQuery callback);
    }
}