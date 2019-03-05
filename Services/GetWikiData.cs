using RandomCity.Models;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RandomCity.Services
{
    public class GetWikiData
    {
        private WebClient client = new WebClient();
        private string wikiApi = "https://en.wikipedia.org/w/api.php";
        
        // Return the text from a wikipedia page
        private async Task<int> GetWikiPageId(City city)
        {
            string subName = city.Subcountry != null ? city.Subcountry : city.Country;
            List<string> uriParams = new List<string> {
                "action=query&", "list=search&", $"srsearch={city.Name}%20{subName}&", "utf8&", "format=json"
                };

            // INSERT ERROR HANDLING HERE
            UriBuilder wikiSearchUri = new UriBuilder(wikiApi);
            uriParams.ForEach(p => wikiSearchUri.Query += p);
            var data = await client.DownloadStringTaskAsync(wikiSearchUri.ToString());
            dynamic searchData = JsonConvert.DeserializeObject(data);
            int pageId = (int)searchData.query.search[0].pageid;
            return pageId;
        }

        private async Task<string> GetWikiPage(City city)
        {
            int pageId = await this.GetWikiPageId(city);
            List<string> uriParams = new List<string> {
                "action=parse&", $"pageid={pageId}&", "utf8&", "format=json"
                };

            UriBuilder wikiPageUri = new UriBuilder(wikiApi);
            uriParams.ForEach(p => wikiPageUri.Query += p);
            var pageData = await client.DownloadStringTaskAsync(wikiPageUri.ToString());
            dynamic pageDataJson = JsonConvert.DeserializeObject(pageData);
            return (string)pageDataJson.parse.text["*"];
        }

        public async Task<int> GetPopulationFallback(City city)
        {
            var wikiPage = await this.GetWikiPage(city);
            string popPattern = @"Population.*?Total</th><td>(.*?)</td>";
            Match popMatch = new Regex(popPattern).Match(wikiPage);
            int rtnVal = popMatch.Groups[1] != null
                ? int.Parse(popMatch.Groups[1].Value, NumberStyles.AllowThousands) : 0;
            return rtnVal;
        }
    }
}