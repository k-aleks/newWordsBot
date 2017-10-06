using System.Collections.Generic;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace NewWordsBot
{
    public class BackgroundQuizHandlerMessanger
    {
        private readonly ITelegramBotClient botClient;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public BackgroundQuizHandlerMessanger(ITelegramBotClient botClient)
        {
            this.botClient = botClient;
        }

        public void SendRightResponse(User user)
        {
            botClient.SendTextMessage(user.ChatId, $"Correct!");
        }

        public void SendWrongReponse(User user)
        {
            botClient.SendTextMessage(user.ChatId, $"Incorrect definition :(");
        }

        public void SendError(User user)
        {
            botClient.SendTextMessage(user.ChatId, $"Oops, some error occured");
        }

        public void AskUser(User user, string word, List<string> definitions, int rightVariantIndex)
        {
            var keyboardButtons = new InlineKeyboardButton[definitions.Count][];
            for (int i = 0; i < definitions.Count; i++)
            {
                keyboardButtons[i] = new[]
                {
                    new InlineKeyboardCallbackButton(definitions[i], $"/response {i} {word}")
                };
            }
            var keyboard = new InlineKeyboardMarkup(keyboardButtons);
            botClient.SendTextMessage(user.ChatId, $"Choose correct definition for the word *{word}*", ParseMode.Markdown, replyMarkup: keyboard);
        }
    }
}