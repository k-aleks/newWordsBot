using Telegram.Bot;
using Telegram.Bot.Types;

namespace NewWordsBot
{
    class HelpHandler : IHandler
    {
        private readonly ITelegramBotClient botClient;

        public HelpHandler(ITelegramBotClient botClient)
        {
            this.botClient = botClient;
        }

        public void Start()
        {
        }

        public bool TryHandleRequest(Message message)
        {
            var usage = @"
ATTENTION: we are run out of available API calls to the dictionary. 
The problem will be solved shortly. 
For more details please reach @akazakov

Usage:
/add <word or expression>  - add new word or expression
";
            botClient.SendTextMessage(message.Chat.Id, usage);
            return true;
        }

        public bool TryHandleCallback(CallbackQuery callback)
        {
            return false;
        }
    }
}