using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NLog;
using Telegram.Bot.Types;

namespace NewWordsBot
{
    internal class BackgroundRiddler : ICommunicator
    {
        private readonly IUsersStorage usersStorage;
        private readonly IWordsStorage wordsStorage;
        private readonly IWordsDictionary dictionary;
        private readonly IPendingRequests pendingRequests;
        private readonly IRandomWordsSelector randomWordsSelector;
        private readonly ILearningMethodology learningMethodology;
        private readonly Random rnd = new Random();
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly BackgroundRiddlerMessanger messanger;

        public BackgroundRiddler(IUsersStorage usersStorage, IWordsStorage wordsStorage, IWordsDictionary dictionary,
            IPendingRequests pendingRequests, IRandomWordsSelector randomWordsSelector, ILearningMethodology learningMethodology, BackgroundRiddlerMessanger messanger)
        {
            this.usersStorage = usersStorage;
            this.wordsStorage = wordsStorage;
            this.dictionary = dictionary;
            this.pendingRequests = pendingRequests;
            this.randomWordsSelector = randomWordsSelector;
            this.learningMethodology = learningMethodology;
            this.messanger = messanger;
        }

        public void Start()
        {
            Task.Run(() => Routine());
        }

        public bool TryHandleCallback(CallbackQuery callback)
        {
            if (!IsOurCallback(callback))
                return false;
            User user = null;
            try
            {
                user = usersStorage.GetOrRegisterUser(callback.Message.Chat.Username);
                int variantIndex;
                string responseForWord;
                ParseCallback(callback, out variantIndex, out responseForWord);
                Word word;
                int rightVariantIndex;
                if (pendingRequests.TryGet(user, out word, out rightVariantIndex))
                {
                    if (word.TheWord == responseForWord)
                    {
                        Word transformatedWordToStore;
                        if (variantIndex == rightVariantIndex)
                        {
                            messanger.SendRightResponse(user);
                            transformatedWordToStore = learningMethodology.OnRightResponse(word);
                        }
                        else
                        {
                            messanger.SendWrongReponse(user);
                            transformatedWordToStore = learningMethodology.OnWrongResponse(word);
                        }
                        wordsStorage.AddOrUpdate(user, transformatedWordToStore);
                        pendingRequests.Remove(user);
                    }
                    else
                    {
                        //just ignore
                    }
                        
                }
                else
                {
                    //just ignore
                }
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

        private static void ParseCallback(CallbackQuery callback, out int variantIndex, out string responseForWord)
        {
            var response = callback.Data.Replace("/response ", "");
            var strings = response.Split(" ", 2);
            variantIndex = int.Parse(strings[0]);
            responseForWord = strings[1];
        }

        private bool IsOurCallback(CallbackQuery callback)
        {
            try
            {
                if (callback.Data.StartsWith("/response"))
                    return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            return false;
        }

        private void Routine()
        {
            while (true)
            {
                RiddleRound();
            }
        }

        private void RiddleRound()
        {
            var users = usersStorage.GetAllUsers();
            foreach (var user in users)
            {
                if (!pendingRequests.ContainsRequest(user))
                {
                    Word word;
                    if ((word = wordsStorage.GetNextReadyToRepeat(user)) != null)
                    {
                        List<string> definitions = GetWrongDefinitions(word);
                        AddWordDefinitionsAtRandomPosition(definitions, word);
                        var rightVariantIndex = definitions.IndexOf(word.Definition);
                        messanger.AskUser(user, word.TheWord, definitions, rightVariantIndex);
                        pendingRequests.Add(user, word, rightVariantIndex);
                    }
                }
                
            }
        }

        private void AddWordDefinitionsAtRandomPosition(List<string> definitions, Word word)
        {
            int rndIndex = rnd.Next(0, definitions.Count);
            string tmp = definitions[rndIndex];
            definitions[rndIndex] = word.Definition;
            definitions.Add(tmp);
        }

        private List<string> GetWrongDefinitions(Word word)
        {
            var defs = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                var w = randomWordsSelector.Select(word.Form);
                defs.Add(dictionary.FindDefinitions(w)[0]); 
            }
            return defs;
        }

        public bool TryHandleRequest(Message message)
        {
            return false;
        }
    }
}