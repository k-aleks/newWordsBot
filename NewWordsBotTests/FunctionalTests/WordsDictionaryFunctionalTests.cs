using FluentAssertions;
using NewWordsBot;
using Xunit;

namespace NewWordsBotTests.FunctionalTests
{
    public class WordsDictionaryFunctionalTests
    {
        [Fact]
        public void Find_assets()
        {
            var dictionaryItem = CreateWordsDictionary().Find("asset");
            
            dictionaryItem.Word.Should().Be("asset");
            dictionaryItem.Definitions.Should().Equal(
                "something such as money or property that a person or company owns",
                "an item of text or media that has been put into a digital form that includes the right to use it",
                "a major benefit");
            dictionaryItem.PartOfSpeech.Should().Be(PartOfSpeech.Noun);
        }
        
        [Fact]
        public void Find_converge()
        {
            var dictionaryItem = CreateWordsDictionary().Find("converge");
            
            dictionaryItem.Word.Should().Be("converge");
            dictionaryItem.Definitions.Should().Equal(
                "to come from different directions to reach the same point",
                "to become the same or very similar");
            dictionaryItem.PartOfSpeech.Should().Be(PartOfSpeech.Verb);
        }
        
        [Fact]
        public void Find_get_rid_of()
        {
            var dictionaryItem = CreateWordsDictionary().Find("get rid of");
            
            dictionaryItem.Word.Should().Be("get rid of");
            dictionaryItem.Definitions.Should().Equal(
                "to be no longer affected by someone or something that is annoying, unpleasant, or not wanted",
                "to throw away, give away, or sell a possession that you no longer want or need",
                "to take action that stops something annoying, unpleasant, or not wanted from affecting you",
                "to make someone go away because they are annoying, unpleasant, or not wanted");
            dictionaryItem.PartOfSpeech.Should().Be(PartOfSpeech.Adjective);
        }

        private static WordsDictionary CreateWordsDictionary()
        {
            var wordsDictionary =
                new WordsDictionary(new MacmillanApiClient(Config.MacmillanApiBaseUrl, Config.MacmillanApiKey));
            return wordsDictionary;
        }
    }
}