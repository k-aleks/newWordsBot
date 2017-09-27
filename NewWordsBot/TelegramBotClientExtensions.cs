using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NewWordsBot
{
    public static class TelegramBotClientExtensions
    {
        public static Message SendTextMessage(this ITelegramBotClient client, ChatId chatId, string text, 
            ParseMode parseMode = ParseMode.Default, 
            bool disableWebPagePreview = false, 
            bool disableNotification = false, 
            int replyToMessageId = 0, 
            IReplyMarkup replyMarkup = null, 
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return client.SendTextMessageAsync(chatId, text, parseMode, disableWebPagePreview, disableNotification,
                replyToMessageId, replyMarkup, cancellationToken).Result;
        }
    }
}