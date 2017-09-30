using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace NewWordsBot
{
    public class NewWordsHandlerMessanger : INewWordsHandlerMessanger
    {
        private readonly ITelegramBotClient botClient;

        public NewWordsHandlerMessanger(ITelegramBotClient botClient)
        {
            this.botClient = botClient;
        }

        public void SendNewWordConfirmation(User user, Word word)
        {
            botClient.SendTextMessage(user.ChatId,
                $"Added new word *{word.TheWord}* with definition _{word.Definition}_", ParseMode.Markdown);
        }

        public void SendError(User user)
        {
            botClient.SendTextMessage(user.ChatId, $"Oops, some error occured");
        }

        public void SendSelectDefinitionRequest(User user, string word, List<string> definitions)
        {
            int defenitionsCount = Math.Min(3, definitions.Count);
            var keyboardButtons = new InlineKeyboardButton[defenitionsCount][];
            for (int i = 0; i < defenitionsCount; i++)
            {
                keyboardButtons[i] = new[]
                {
                    new InlineKeyboardCallbackButton(definitions[i], "/add " + i)
                };
            }
            var keyboard = new InlineKeyboardMarkup(keyboardButtons);
            botClient.SendTextMessage(user.ChatId, $"Choose definition for the word \"{word}\"", replyMarkup: keyboard);
        }
    }
}