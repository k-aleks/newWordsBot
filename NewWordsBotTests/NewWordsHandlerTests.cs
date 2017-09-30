using System;
using System.Collections.Generic;
using FluentAssertions;
using NewWordsBot;
using NSubstitute;
using Telegram.Bot.Types;
using Xunit;

namespace NewWordsBotTests
{
    //TODO: refactoring
    public class NewWordsHandlerTests
    {
        [Fact]
        public void TryHandleRequest_should_return_false_for_nonAdd_request()
        {
            var handler = new NewWordsHandler(MockUsersStorage(), MockWordsStorage(), MockWordsDictionary(), MockNewWordsHandlerMessanger(), MockLearningMethodology());

            Message message = new Message
            {
                Text = "/notadd"
            };

            handler.TryHandleRequest(message).Should().BeFalse();
        }


        [Fact]
        public void TryHandleRequest_should_send_correct_request_to_user_and_return_true()
        {
            var wordToLearn = "word to learn";
            var definitions = new List<string>()
            {
                "definition one",
                "definition two",
                "definition three",
            };
            var user = new NewWordsBot.User("myUserName", 12345);

            var usersStorage = MockUsersStorage();
            usersStorage.GetOrRegisterUser(null).ReturnsForAnyArgs(user);

            var dictionary = MockWordsDictionary();
            dictionary.Find(wordToLearn).Returns(new DictionaryItem(wordToLearn, definitions, WordForm.Noun));

            var messanger = MockNewWordsHandlerMessanger();
            
            var handler = new NewWordsHandler(usersStorage, MockWordsStorage(), dictionary, messanger, MockLearningMethodology());

            Message message = new Message
            {
                Text = "/add " + wordToLearn,
            };

            handler.TryHandleRequest(message).Should().BeTrue();
            messanger.Received().SendSelectDefinitionRequest(user, wordToLearn, definitions);
        }

        [Fact]
        public void TryHandleCallback_should_return_false_if_no_pending_definitions()
        {
            var handler = new NewWordsHandler(MockUsersStorage(), MockWordsStorage(), MockWordsDictionary(), MockNewWordsHandlerMessanger(), MockLearningMethodology());

            CallbackQuery callback = new CallbackQuery()
            {
               Data = "/notadd"
            };
            
            handler.TryHandleCallback(callback).Should().BeFalse();
        }
        
        [Fact]
        public void TryHandleCallback_should_call_methodology_for_creating_new_word_and_save_word_to_storage_and_return_true()
        {
            var wordToLearn = "word to learn";
            var definitions = new List<string>()
            {
                "definition one",
                "definition two",
                "definition three",
            };
            var user = new NewWordsBot.User("myUserName", 12345);
            var word = new Word(wordToLearn, definitions[1], WordForm.Noun, LearningStage.First_1m, DateTime.MinValue, DateTime.Now);

            var usersStorage = MockUsersStorage();
            usersStorage.GetOrRegisterUser(null).ReturnsForAnyArgs(user);

            var dictionary = MockWordsDictionary();
            dictionary.Find(wordToLearn).Returns(new DictionaryItem(wordToLearn, definitions, WordForm.Noun));

            var messanger = MockNewWordsHandlerMessanger();

            var learningMethodology = MockLearningMethodology();
            learningMethodology.CreateNewWord(wordToLearn, definitions[1], WordForm.Noun).Returns(word);
            var wordsStorage = MockWordsStorage();
            var newWordsHandler = new NewWordsHandler(usersStorage, wordsStorage, dictionary, messanger, learningMethodology);
            var handler = newWordsHandler;

            Message message = new Message
            {
                Text = "/add " + wordToLearn,
            };

            handler.TryHandleRequest(message).Should().BeTrue();

            CallbackQuery callback = new CallbackQuery()
            {
               Data = "/add 1",
               Message = new Message()
            };
            
            handler.TryHandleCallback(callback).Should().BeTrue();
            learningMethodology.Received().CreateNewWord(wordToLearn, definitions[1], WordForm.Noun);
            wordsStorage.Received().AddOrUpdate(user, word);
            messanger.Received().SendNewWordConfirmation(user, word);
        }
        
        [Fact]
        public void TryHandleCallback_should_not_do_nothing_on_second_callback_and_return_false()
        {
            var wordToLearn = "word to learn";
            var definitions = new List<string>()
            {
                "definition one",
                "definition two",
                "definition three",
            };
            var user = new NewWordsBot.User("myUserName", 12345);
            var word = new Word(wordToLearn, definitions[1], WordForm.Noun, LearningStage.First_1m, DateTime.MinValue, DateTime.Now);

            var usersStorage = MockUsersStorage();
            usersStorage.GetOrRegisterUser(null).ReturnsForAnyArgs(user);

            var dictionary = MockWordsDictionary();
            dictionary.Find(wordToLearn).Returns(new DictionaryItem(wordToLearn, definitions, WordForm.Noun));

            var messanger = MockNewWordsHandlerMessanger();

            var learningMethodology = MockLearningMethodology();
            learningMethodology.CreateNewWord(wordToLearn, definitions[1], WordForm.Noun).Returns(word);
            var wordsStorage = MockWordsStorage();
            var newWordsHandler = new NewWordsHandler(usersStorage, wordsStorage, dictionary, messanger, learningMethodology);
            var handler = newWordsHandler;

            Message message = new Message
            {
                Text = "/add " + wordToLearn,
            };

            handler.TryHandleRequest(message).Should().BeTrue();

            CallbackQuery callback = new CallbackQuery()
            {
               Data = "/add 1",
               Message = new Message()
            };
            
            handler.TryHandleCallback(callback).Should().BeTrue();
            
            learningMethodology.ClearReceivedCalls();
            wordsStorage.ClearReceivedCalls();
            messanger.ClearReceivedCalls();
            
            handler.TryHandleCallback(callback).Should().BeFalse();
            
            learningMethodology.DidNotReceiveWithAnyArgs().CreateNewWord(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<WordForm>());
            wordsStorage.DidNotReceiveWithAnyArgs().AddOrUpdate(Arg.Any<NewWordsBot.User>(), Arg.Any<Word>());
            messanger.DidNotReceiveWithAnyArgs().SendNewWordConfirmation(Arg.Any<NewWordsBot.User>(), Arg.Any<Word>());
        }
        
        private static ILearningMethodology MockLearningMethodology()
        {
            return Substitute.For<ILearningMethodology>();
        }

        private static INewWordsHandlerMessanger MockNewWordsHandlerMessanger()
        {
            return Substitute.For<INewWordsHandlerMessanger>();
        }

        private static IUsersStorage MockUsersStorage()
        {
            return Substitute.For<IUsersStorage>();
        }

        private static IWordsDictionary MockWordsDictionary()
        {
            return Substitute.For<IWordsDictionary>();
        }

        private static IWordsStorage MockWordsStorage()
        {
            return Substitute.For<IWordsStorage>();
        }
    }
}