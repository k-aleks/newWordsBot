using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace NewWordsBot
{
    public class MacmillanApiClient
    {
        private readonly string baseUri;
        private readonly string apiKey;
        private readonly HttpClient httpClient;

        public MacmillanApiClient(string baseUri, string apiKey)
        {
            this.baseUri = baseUri;
            this.apiKey = apiKey;
            httpClient = new HttpClient();
        }

        public string Search(string searchQuery)
        {
            var queryString = HttpUtility.HtmlEncode("q=" + searchQuery);
            var requestUri = $"{baseUri}/dictionaries/british/search?{queryString}";
            return Get(requestUri);
        }

        public string GetItem(string itemId)
        {
            var queryString = HttpUtility.HtmlEncode("format=xml");
            var requestUri = $"{baseUri}/dictionaries/british/entries/{itemId}?{queryString}";
            return Get(requestUri);
        }

        private string Get(string requestUri)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Headers.Add("accessKey", apiKey);
            var httpResponseMessage = httpClient.SendAsync(httpRequest).Result;
            return httpResponseMessage.Content.ReadAsStringAsync().Result;
        }
    }
}