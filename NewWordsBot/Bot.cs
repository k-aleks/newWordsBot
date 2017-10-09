using System;
using System.Collections.Generic;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace NewWordsBot
{
    class Bot
    {
        private readonly ITelegramBotClient botClient;
        private readonly List<IHandler> handlers;
        private readonly ILog logger = LogManager.GetLogger(typeof(Bot));

        public Bot(ITelegramBotClient botClient, List<IHandler> handlers)
        {
            this.botClient = botClient;
            this.handlers = handlers;
        }

        public void Start()
        {
            foreach (var handler in handlers)
            {
                handler.Start();
            }
            
            var getMeResult = botClient.GetMeAsync().Result;
            logger.Info($"Received bot user with username {getMeResult.Username}");

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
            try
            {
                var message = e.Message;
                foreach (var handler in handlers)
                {
                    if (handler.TryHandleRequest(message))
                        return;
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void OnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            try
            {
                CallbackQuery callback = e.CallbackQuery;
                foreach (var handler in handlers)
                {
                    if (handler.TryHandleCallback(callback))
                        return;
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
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