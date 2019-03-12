using RandomCityApi.Models;
using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RandomCityApi.Services
{
    public class WikiCityData
    {
        public int area;
        public string summary;
        public int population;
        public string wikiRef;
    }
    public class GetWikiData
    {
        private WebClient client = new WebClient();
        private ProcessWikiData processData = new ProcessWikiData();
        private string wikiApi = "https://en.wikipedia.org/w/api.php";
        private string wikiUrl = "https://en.wikipedia.org/";
        
        // Return the text from a wikipedia page
        private async Task<int> GetWikiPageId(City city)
        {
            string subName = city.Subcountry != null ? city.Subcountry : city.Country;
            List<string> uriParams = new List<string> {
                "action=query&", "list=search&", $"srsearch={city.Name}%20{subName}&", "utf8&", "format=json"
                };

            UriBuilder wikiSearchUri = new UriBuilder(wikiApi);
            uriParams.ForEach(p => wikiSearchUri.Query += p);
            try
            {
                var data = await client.DownloadStringTaskAsync(wikiSearchUri.ToString());
                dynamic searchData = JsonConvert.DeserializeObject(data);
                int pageId = (int)searchData.query.search[0].pageid;
                return pageId;
            } catch(Exception ex)
            {
                System.Console.WriteLine(ex);
                return 0;
            }
        }

        private async Task<string> GetWikiPage(int pageId)
        {
            if (pageId == 0)
            {
                return null;
            }

            List<string> uriParams = new List<string> {
                "action=parse&", $"pageid={pageId}&", "utf8&", "format=json"
                };

            UriBuilder wikiPageUri = new UriBuilder(wikiApi);
            uriParams.ForEach(p => wikiPageUri.Query += p);
            try
            {
                var pageData = await client.DownloadStringTaskAsync(wikiPageUri.ToString());
                dynamic pageDataJson = JsonConvert.DeserializeObject(pageData);
                return (string)pageDataJson.parse.text["*"];
            } catch(Exception ex)
            {
                System.Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<WikiCityData> GetWikiCityData(City city)
        {
            WikiCityData wikiData = new WikiCityData();
            int pageId = await this.GetWikiPageId(city);
            var wikiPage = await this.GetWikiPage(pageId);
            if (wikiPage == null)
            {
                wikiData.area = 0;
                wikiData.summary = null;
            }
            else 
            {
                wikiData.area = processData.MatchArea(wikiPage);
                wikiData.summary = processData.MatchSummary(wikiPage);
                wikiData.wikiRef = $"{this.wikiUrl}?curid={pageId}";
            }

            return wikiData;
        }

        public async Task<WikiCityData> GetPopulationFallback(City city)
        {
            int pageId = await this.GetWikiPageId(city);
            var wikiPage = await this.GetWikiPage(pageId);
            if (wikiPage == null)
            {
                return null;
            }

            WikiCityData wikiData = new WikiCityData();
            wikiData.population = processData.MatchPopulation(wikiPage);
            wikiData.wikiRef = $"{this.wikiUrl}?curid={pageId}";
            return wikiData;
        }
    }
}