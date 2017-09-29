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
    class NewWordsDefinitor : ICommunicator
    {
        private readonly Dictionary<string, Tuple<string, List<string>>> pendingDefinitions = new Dictionary<string, Tuple<string, List<string>>>();
        private readonly ITelegramBotClient botClient;
        private readonly IUsersStorage usersStorage;
        private readonly IWordsStorage wordsStorage;
        private readonly IWordsDictionary dictionary;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public NewWordsDefinitor(IUsersStorage usersStorage, IWordsStorage wordsStorage, IWordsDictionary dictionary, ITelegramBotClient botClient)
        {
            this.usersStorage = usersStorage;
            this.wordsStorage = wordsStorage;
            this.dictionary = dictionary;
            this.botClient = botClient;
        }

        public bool TryHandleRequest(Message message)
        {
            throw new NotImplementedException();
        }

        public bool TryHandleCallback(CallbackQuery callback)
        {
            var message = callback.Message;
            try
            {
                var user = usersStorage.GetOrRegisterUser(message.Chat);
                var callbackData = callback.Data;
            
                logger.Debug($"New callback query from {user.Username} with callback data: {callbackData}");

                if (callbackData.StartsWith("/add"))
                {
                    if (!pendingDefinitions.ContainsKey(user.Username))
                    {
                        logger.Warn($"No pending definitions. User {user.Username}, callback data: {callbackData}");
                        return false;
                    }
                    var pendingDefinition = pendingDefinitions[user.Username];
                    var word = pendingDefinition.Item1;
                    var definition = pendingDefinition.Item2[int.Parse(callbackData.Split(' ')[1])];
                    wordsStorage.AddOrUpdate(user, new Word(word, definition, ));
                
                    logger.Info($"Added new word \"{word}\" with definition \"{definition}\"");
                    botClient.SendTextMessage(message.Chat.Id, $"Added new word *{word}* with definition _{definition}_", ParseMode.Markdown);
                    pendingDefinitions.Remove(user.Username);
                    return true;
                }
                else
                {
                    logger.Warn("Unexpected callback");
                    botClient.SendTextMessage(message.Chat.Id, $"Unexpected callback, sorry");
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                botClient.SendTextMessage(message.Chat.Id, "Ooops, some error");
                return false;
            }
        }
    }
    
    class Bot
    {
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

            new BackgroundRiddler(usersStorage, wordsStorage, dictionary).Start();
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
                    var word = message.Text.Replace("/add", "").Trim();
                    List<string> definitions = dictionary.FindDefinitions(word);
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
                    botClient.SendTextMessage(message.Chat.Id, $"Choose definition for the word \"{word}\"", replyMarkup: keyboard);
                    pendingDefinitions[user.Username] = new Tuple<string, List<string>>(word, definitions);
                    return;
                }
            
                var usage = @"Usage:
/add <word or expression>  - add new word or expression
";
                botClient.SendTextMessage(message.Chat.Id, usage);
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                botClient.SendTextMessage(message.Chat.Id, "Ooops, some error");
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
            CallbackQuery callback = e.CallbackQuery;
            var message = callback.Message;
            try
            {
                var user = usersStorage.GetOrRegisterUser(message.Chat.Username);
                var callbackData = callback.Data;
            
                logger.Debug($"New callback query from {user.Username} with callback data: {callbackData}");

                if (callbackData.StartsWith("/add"))
                {
                    if (!pendingDefinitions.ContainsKey(user.Username))
                    {
                        logger.Warn($"No pending definitions. User {user.Username}, callback data: {callbackData}");
                        return;
                    }
                    var pendingDefinition = pendingDefinitions[user.Username];
                    var word = pendingDefinition.Item1;
                    var definition = pendingDefinition.Item2[int.Parse(callbackData.Split(' ')[1])];
                    wordsStorage.AddOrUpdate(user, word, definition);
                
                    logger.Info($"Added new word \"{word}\" with definition \"{definition}\"");
                    botClient.SendTextMessage(message.Chat.Id, $"Added new word *{word}* with definition _{definition}_", ParseMode.Markdown);
                    pendingDefinitions.Remove(user.Username);
                }
                else
                {
                    logger.Warn("Unexpected callback");
                    botClient.SendTextMessage(message.Chat.Id, $"Unexpected callback, sorry");
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                botClient.SendTextMessage(message.Chat.Id, "Ooops, some error");
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