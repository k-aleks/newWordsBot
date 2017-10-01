using System.Collections.Generic;
using FluentAssertions;
using NewWordsBot;
using Xunit;

namespace NewWordsBotTests
{
    public class MacmillanGetItemResultParserTests
    {
        [Fact]
        public void TryParse_should_successfuly_parse_result_for_word_asset()
        {
            var response = System.IO.File.ReadAllText("../../../../NewWordsBot/examples/asset-original-response.json");
            
            List<string> definitions;
            PartOfSpeech partOfSpeech;
            new MacmillanGetItemResultParser().TryParse(response, out definitions, out partOfSpeech).Should().BeTrue();

            partOfSpeech.Should().Be(PartOfSpeech.Noun);
            definitions.Should().Equal(
                "something such as money or property that a person or company owns",
                "an item of text or media that has been put into a digital form that includes the right to use it",
                "a major benefit");
        }
        
        [Fact]
        public void TryParse_should_successfuly_parse_result_for_word_converge()
        {
            var response = System.IO.File.ReadAllText("../../../../NewWordsBot/examples/converge-original-response.json");
            
            List<string> definitions;
            PartOfSpeech partOfSpeech;
            new MacmillanGetItemResultParser().TryParse(response, out definitions, out partOfSpeech).Should().BeTrue();

            partOfSpeech.Should().Be(PartOfSpeech.Verb);
            definitions.Should().Equal(
                "to come from different directions to reach the same point",
                "to become the same or very similar");
        }
        
        [Fact]
        public void TryParse_should_successfuly_parse_result_for_phrase_get_rid_of()
        {
            var response = System.IO.File.ReadAllText("../../../../NewWordsBot/examples/get+rid+of-original-response.json");
            
            List<string> definitions;
            PartOfSpeech partOfSpeech;
            new MacmillanGetItemResultParser().TryParse(response, out definitions, out partOfSpeech).Should().BeTrue();

            partOfSpeech.Should().Be(PartOfSpeech.Adjective);
            definitions.Should().Equal(
                "to be no longer affected by someone or something that is annoying, unpleasant, or not wanted",
                "to throw away, give away, or sell a possession that you no longer want or need",
                "to take action that stops something annoying, unpleasant, or not wanted from affecting you",
                "to make someone go away because they are annoying, unpleasant, or not wanted");
        }
        
    }
}