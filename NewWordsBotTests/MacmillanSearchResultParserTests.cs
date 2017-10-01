using FluentAssertions;
using NewWordsBot;
using Xunit;

namespace NewWordsBotTests
{
    public class MacmillanSearchResultParserTests
    {
        [Fact]
        public void TryParse_should_successfuly_parse_result_for_word_asset()
        {
            var response = System.IO.File.ReadAllText("../../../../NewWordsBot/examples/asset-search-response.json");
            
            string itemId;
            new MacmillanSearchResultParser().TryParse(response, out itemId).Should().BeTrue();
            itemId.Should().Be("asset");
        }
        
        [Fact]
        public void TryParse_should_successfuly_parse_result_for_word_converge()
        {
            var response = System.IO.File.ReadAllText("../../../../NewWordsBot/examples/converge-search-response.json");
            
            string itemId;
            new MacmillanSearchResultParser().TryParse(response, out itemId).Should().BeTrue();
            itemId.Should().Be("converge");
        }
        
        [Fact]
        public void TryParse_should_successfuly_parse_result_for_phrase_get_rid_of()
        {
            var response = System.IO.File.ReadAllText("../../../../NewWordsBot/examples/get+rid+of-search-response.json");
            
            string itemId;
            new MacmillanSearchResultParser().TryParse(response, out itemId).Should().BeTrue();
            itemId.Should().Be("rid_1");
        }
    }
}