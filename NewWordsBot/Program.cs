using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using MongoDB.Bson;
using MongoDB.Driver;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

namespace NewWordsBot
{
    class Program
    {
        private static int counter = 0;
        static TelegramBotClient botClient = new TelegramBotClient(Config.Token);
        
        static void Main(string[] args)
        {
            var getMeResult = botClient.GetMeAsync().Result;
            Console.Out.WriteLine($"Bot's first name: {getMeResult.FirstName}");
            Console.Out.WriteLine($"Bot's last name: {getMeResult.LastName}");

            botClient.OnMessage += OnMessageReceived;
            botClient.OnMessageEdited += OnMessageReceived;
            botClient.OnCallbackQuery += OnCallbackQueryReceived;
            botClient.OnInlineQuery += OnInlineQueryReceived;
            botClient.OnInlineResultChosen += OnChosenInlineResultReceived;
            botClient.OnReceiveError += OnReceiveError;


            botClient.StartReceiving();
            Console.Out.WriteLine("Started");
            Console.ReadLine();
            botClient.StopReceiving();
            Console.Out.WriteLine("Stopped");
        }

        private static void OnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.Out.WriteLine("Error received");
        }

        private static void OnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received choosen inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }
        
        private static async void OnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            InlineQueryResult[] results = {
                new InlineQueryResultLocation
                {
                    Id = "1",
                    Latitude = 40.7058316f, // displayed result
                    Longitude = -74.2581888f,
                    Title = "New York",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Latitude = 40.7058316f,
                        Longitude = -74.2581888f,
                    }
                },

                new InlineQueryResultLocation
                {
                    Id = "2",
                    Longitude = 52.507629f, // displayed result
                    Latitude = 13.1449577f,
                    Title = "Berlin",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Longitude = 52.507629f,
                        Latitude = 13.1449577f
                    }
                }
            };

            await botClient.AnswerInlineQueryAsync(inlineQueryEventArgs.InlineQuery.Id, results, isPersonal: true, cacheTime: 0);
        }
        
        private static async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            Console.Out.WriteLine("On message received " + counter++);
            var message = e.Message;
            
            if (message == null)
            {
                Console.Out.WriteLine($"Message is null");
                return;
            }
            if (message == null || message.Type != MessageType.TextMessage)
            {
                Console.Out.WriteLine($"Unexpected type of message: {message.Type}");
                return;
            }
            var username = message.Chat.Username;
            Console.Out.WriteLine($"New message from {username}");
            Console.Out.WriteLine(message.Text);

            if (message.Text.StartsWith("/inline")) // send inline keyboard
            {
                await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] // first row
                    {
                        new InlineKeyboardCallbackButton("1.1", "1.1"), 
                        new InlineKeyboardCallbackButton("1.2", "1.2"), 
                    },
                    new[] // second row
                    {
                        new InlineKeyboardCallbackButton("2.1", "2.1"), 
                        new InlineKeyboardCallbackButton("2.2", "2.2"), 
                    }
                });

                await botClient.SendTextMessageAsync(message.Chat.Id, "Choose", replyMarkup: keyboard);
            }
            else if (message.Text.StartsWith("/keyboard")) // send custom keyboard
            {
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new [] // first row
                    {
                        new KeyboardButton("1.1"),
                        new KeyboardButton("1.2"),  
                    },
                    new [] // last row
                    {
                        new KeyboardButton("2.1"),
                        new KeyboardButton("2.2"),  
                    }
                });

                await botClient.SendTextMessageAsync(message.Chat.Id, "Choose", replyMarkup: keyboard);
            }
            else if (message.Text.StartsWith("/request")) // request location or contact
            {
                var keyboard = new ReplyKeyboardMarkup(new []
                {
                    new KeyboardButton("Location")
                    {
                        RequestLocation = true
                    },
                    new KeyboardButton("Contact")
                    {
                        RequestContact = true
                    }, 
                });

                await botClient.SendTextMessageAsync(message.Chat.Id, "Who or Where are you?", replyMarkup: keyboard);
            }
            else if (message.Text.StartsWith("/testdb")) // test access to db
            {
                var client = new MongoClient(Config.MongoDbConnectionString);
                var database = client.GetDatabase(Config.DatabaseName);
                var collection = database.GetCollection<BsonDocument>(Config.UsersCollection);
                var newUserName = username + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                await collection.InsertOneAsync(new BsonDocument("Name", newUserName));
                var list = await collection.Find(new BsonDocument()).ToListAsync();
                string allusers = string.Join("\r\n", list.Select(d => d["Name"]));
                
                await botClient.SendTextMessageAsync(message.Chat.Id, allusers, replyMarkup: new ReplyKeyboardRemove());
            }
            else
            {
                var usage = @"Usage:
/inline   - send inline keyboard
/keyboard - send custom keyboard
/request  - request location or contact
/testdb  - test mongodb connection
";
                await botClient.SendTextMessageAsync(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardRemove());
            }
        }
        
        private static async void OnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id,
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
            await botClient.SendTextMessageAsync(callbackQueryEventArgs.CallbackQuery.Message.Chat.Id, "You choosed " + callbackQueryEventArgs.CallbackQuery.Data, replyMarkup: new ReplyKeyboardRemove());
        }
    }
}