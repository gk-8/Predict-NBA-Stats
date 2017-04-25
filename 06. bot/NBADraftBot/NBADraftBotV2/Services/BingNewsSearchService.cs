using NBADraftBotV2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NBADraftBotV2.Services
{
    public class BingNewsSearchService
    {
        public async Task<List<NewsValue>> GetPlayerNewsByName(string playerName) {
            BingNewsSearchResult result = await SearchNews(playerName);
            return result.value;
        }
        private async Task<BingNewsSearchResult> SearchNews(string playerName)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{your-subscription-key}");

            // Request parameters
            queryString["q"] = playerName;
            queryString["count"] = "5";
            queryString["offset"] = "0";
            queryString["mkt"] = "en-us";
            queryString["safeSearch"] = "Moderate";
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/news/search?" + queryString;

            var response = await client.GetAsync(uri);
            string content = await response.Content.ReadAsStringAsync();
            BingNewsSearchResult result = JsonConvert.DeserializeObject<BingNewsSearchResult>(content);
            return result;
        }

    }
}