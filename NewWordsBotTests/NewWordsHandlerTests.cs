using System.Collections.Generic;
using FluentAssertions;
using NewWordsBot;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace NewWordsBotTests
{
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
           Assert.True(false, "TBD"); 
        }
        
        [Fact]
        public void TryHandleCallback_should_call_methodology_for_creating_new_word_and_save_word_to_storage_and_return_true()
        {
           Assert.True(false, "TBD"); 
        }
        
        [Fact]
        public void TryHandleCallback_should_not_do_nothing_on_second_callback_and_return_false()
        {
           Assert.True(false, "TBD"); 
        }
        
        private static ILearningMethodology MockLearningMethodology()
        {
            ILearningMethodology learningMethodology = Substitute.For<ILearningMethodology>();
            return learningMethodology;
        }

        private static INewWordsHandlerMessanger MockNewWordsHandlerMessanger()
        {
            INewWordsHandlerMessanger messanger = Substitute.For<INewWordsHandlerMessanger>();
            return messanger;
        }

        private static IUsersStorage MockUsersStorage()
        {
            var usersStorage = Substitute.For<IUsersStorage>();
            return usersStorage;
        }

        private static IWordsDictionary MockWordsDictionary()
        {
            IWordsDictionary dictionary = Substitute.For<IWordsDictionary>();
            return dictionary;
        }

        private static IWordsStorage MockWordsStorage()
        {
            var wordsStorage = Substitute.For<IWordsStorage>();
            return wordsStorage;
        }
    }
}