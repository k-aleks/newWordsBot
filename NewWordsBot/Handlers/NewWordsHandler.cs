using System;
using System.Collections.Generic;
using log4net;
using Telegram.Bot.Types;

namespace NewWordsBot
{
    public class NewWordsHandler : IHandler
    {
        private readonly Dictionary<string, DictionaryItem> pendingDefinitions = new Dictionary<string, DictionaryItem>();
        private readonly IUsersStorage usersStorage;
        private readonly IWordsStorage wordsStorage;
        private readonly IWordsDictionary dictionary;
        private readonly ILearningMethodology learningMethodology;
        private readonly ILog logger = LogManager.GetLogger(typeof(NewWordsHandler));
        private readonly INewWordsHandlerMessanger messanger;

        public NewWordsHandler(IUsersStorage usersStorage, IWordsStorage wordsStorage, IWordsDictionary dictionary, INewWordsHandlerMessanger messanger, ILearningMethodology learningMethodology)
        {
            this.usersStorage = usersStorage;
            this.wordsStorage = wordsStorage;
            this.dictionary = dictionary;
            this.messanger = messanger;
            this.learningMethodology = learningMethodology;
        }

        public void Start()
        {
        }

        public bool TryHandleRequest(Message message)
        {
            if (!IsOurRequest(message))
                return false;
            User user = null;
            try
            {
                user = usersStorage.GetOrRegisterUser(message.Chat);
                var wordToLearn = message.Text.Replace("/add", "").Trim();
                var dictionaryItem = dictionary.Find(wordToLearn);
                messanger.SendSelectDefinitionRequest(user, dictionaryItem.Word, dictionaryItem.Definitions);
                pendingDefinitions[user.Username] = dictionaryItem;
                return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
                if (user != null)
                    messanger.SendError(user);
            }
            return false;
        }

        //TODO: don't handle callback for another word
        public bool TryHandleCallback(CallbackQuery callback)
        {
            if (!IsOurCallback(callback))
                return false;
            User user = null;
            try
            {
                user = usersStorage.GetOrRegisterUser(callback.Message.Chat);
                
                var callbackData = callback.Data;
            
                logger.Debug($"New callback query from {user.Username} with callback data: {callbackData}");

                if (!pendingDefinitions.ContainsKey(user.Username))
                {
                    logger.Warn($"No pending definitions. User {user.Username}, callback data: {callbackData}");
                    return false;
                }
                var dicItem = pendingDefinitions[user.Username];
                var definitionIndex = int.Parse(callbackData.Split(' ')[1]);

                var word = learningMethodology.CreateNewWord(dicItem.Word, dicItem.Definitions[definitionIndex], dicItem.PartOfSpeech);
                
                wordsStorage.AddOrUpdate(user, word);
                
                messanger.SendNewWordConfirmation(user, word);
                logger.Info($"Added new word {word.TheWord} with definition {word.Definition}");
                
                pendingDefinitions.Remove(user.Username);
                return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
                if (user != null)
                    messanger.SendError(user);
            }
            return false;
        }
    
        private bool IsOurRequest(Message message)
        {
            try
            {
                if (message.Text.StartsWith("/add"))
                    return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            return false;
        }
        
        private bool IsOurCallback(CallbackQuery callback)
        {
            try
            {
                if (callback.Data.StartsWith("/add"))
                    return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            return false;
        }
    }
}