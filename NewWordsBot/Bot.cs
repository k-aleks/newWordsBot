using System;
using System.Collections.Generic;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace NewWordsBot
{
    class Bot
    {
        private readonly Dictionary<string, Tuple<string, List<string>>> pendingDefinitions = new Dictionary<string, Tuple<string, List<string>>>();
        private readonly ITelegramBotClient botClient;
        private readonly IUsersStorage usersStorage;
        private readonly IWordsStorage wordsStorage;
        private readonly IWordsDictionary dictionary;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public Bot(ITelegramBotClient botClient, IUsersStorage usersStorage, IWordsStorage wordsStorage, IWordsDictionary dictionary)
        {
            this.botClient = botClient;
            this.usersStorage = usersStorage;
            this.wordsStorage = wordsStorage;
            this.dictionary = dictionary;
        }

        public void Start()
        {
            var getMeResult = botClient.GetMeAsync().Result;
            logger.Info("Received bot user with username {0}", getMeResult.Username);

            botClient.OnMessage += OnMessageReceived;
            botClient.OnCallbackQuery += OnCallbackQueryReceived;
            botClient.OnInlineQuery += OnInlineQueryReceived;
            botClient.OnInlineResultChosen += OnChosenInlineResultReceived;
            botClient.OnReceiveError += OnReceiveError;

            botClient.StartReceiving();
            logger.Info("Bot started");
        }
        
        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            try
            {
                if (!MessageIsValid(message))
                    return;
                var user = usersStorage.GetOrRegisterUser(message.Chat.Username);
            
                logger.Debug($"New message from {user.Username} with text: {message.Text}");
            

                if (message.Text.StartsWith("/add"))
                {
                    var word = message.Text.Replace("/add", "");
                    List<string> definitions = dictionary.FindDefinitions(word);
                    int defenitionsCount = Math.Min(3, definitions.Count);
                    var keyboardButtons = new InlineKeyboardButton[defenitionsCount][];
                    for (int i = 0; i < defenitionsCount; i++)
                    {
                        keyboardButtons[i] = new[]
                        {
                            new InlineKeyboardCallbackButton(definitions[i], i.ToString())
                        };
                    }
                    var keyboard = new InlineKeyboardMarkup(keyboardButtons);
                    botClient.SendTextMessage(message.Chat.Id, "Choose", replyMarkup: keyboard);
                    pendingDefinitions[user.Username] = new Tuple<string, List<string>>(word, definitions);
                    return;
                }
            
                var usage = @"Usage:
/add <word or expression>  - add new word or expression
";
                botClient.SendTextMessage(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardRemove());
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                botClient.SendTextMessage(message.Chat.Id, "Ooops, some error", replyMarkup: new ReplyKeyboardRemove());
            }
        }

        private bool MessageIsValid(Message message)
        {
            if (message == null)
            {
                logger.Warn("Message is null");
                return false;
            }
            if (message.Type != MessageType.TextMessage)
            {
                logger.Warn($"Unexpected type of message: {message.Type}");
                return false;
            }
            return true;
        }

        private void OnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var message = e.CallbackQuery.Message;
            try
            {
                var user = usersStorage.GetOrRegisterUser(message.Chat.Username);
                var callbackData = e.CallbackQuery.Data;
            
                logger.Debug($"New callback query from {user.Username} with text: {callbackData}");

                var pendingDefinition = pendingDefinitions[user.Username];
                var word = pendingDefinition.Item1;
                var definition = pendingDefinition.Item2[int.Parse(callbackData)];
                wordsStorage.AddNewWord(user, word, definition);
            
                botClient.SendTextMessage(message.Chat.Id, $"Added new word \"{word}\" with definition \"{definition}\"", replyMarkup: new ReplyKeyboardRemove());
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                botClient.SendTextMessage(message.Chat.Id, "Ooops, some error", replyMarkup: new ReplyKeyboardRemove());
            }
        }

        private void OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnInlineQueryReceived(object sender, InlineQueryEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}