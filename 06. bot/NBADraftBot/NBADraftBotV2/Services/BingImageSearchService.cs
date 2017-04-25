using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using NBADraftBotV2.Models;

namespace NBADraftBotV2.Services
{
    public class BingImageSearchService
    {
        public async Task<string> GetPlayerPhotoByName(string playerName) {
            string imageUrl = "";
            BingImageSearchResult result = await SearchPhoto(playerName);
            if (result != null && result.value != null && result.value.Count > 0) {
                imageUrl = result.value.FirstOrDefault().contentUrl;
            }
            return imageUrl;
        }
        private async Task<BingImageSearchResult> SearchPhoto(string playerName) {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{your-subscription-key}");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Request parameters
            queryString["q"] = playerName;
            queryString["count"] = "1";
            queryString["offset"] = "0";
            queryString["mkt"] = "en-us";
            queryString["safeSearch"] = "Moderate";
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/images/search?" + queryString;

            var response = await client.GetAsync(uri);
            string content = await response.Content.ReadAsStringAsync();
            BingImageSearchResult result = JsonConvert.DeserializeObject<BingImageSearchResult>(content);
            return result;
        }
    }
}