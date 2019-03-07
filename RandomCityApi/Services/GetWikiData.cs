using RandomCityApi.Models;
using System;
using System.Text;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RandomCityApi.Services
{
    public class WikiCityData
    {
        public int area;
        public string summary;
    }
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

        private async Task<string> GetWikiPage(City city)
        {
            int pageId = await this.GetWikiPageId(city);
            if (pageId == 0)
            {
                return null;
            }
            System.Console.WriteLine($"Wiki page id is: {pageId}");

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
            var wikiPage = await this.GetWikiPage(city);
            if (wikiPage == null)
            {
                // Do something that makes sense here
            }

            // Get and process area
            string areaPattern = @"Area.*?Total</th><td>(.*?)\&.*?(km|mi)";
            Match areaMatch = new Regex(areaPattern).Match(wikiPage);
            if (areaMatch.Success && areaMatch.Groups[1] != null && areaMatch.Groups[2] != null)
            {
                double area = double.Parse(areaMatch.Groups[1].Value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
                string unit = areaMatch.Groups[2].Value;
                double areaConvert = unit == "km" ? area * 0.62 : area;
                wikiData.area = Convert.ToInt32(areaConvert);
            }

            // Get and process summary
            string summaryPattern = @"<p>(.*?)\n";
            Match summaryMatch = new Regex(summaryPattern).Match(wikiPage);
            if (summaryMatch.Success && summaryMatch.Groups[1] != null)
            {
                string citySummary = summaryMatch.Groups[1].Value;
                // Not quite on &#123;
                string formattedSummary = Regex.Replace(citySummary, "<.*?>|&#/d*?;", String.Empty);
                wikiData.summary = formattedSummary;
            }

            return wikiData;
        }

        public async Task<int> GetPopulationFallback(City city)
        {
            var wikiPage = await this.GetWikiPage(city);
            if (wikiPage == null)
            {
                return 0;
            }

            string popPattern = @"Population.*?Total</th><td>(.*?)</td>";
            Match popMatch = new Regex(popPattern).Match(wikiPage);
            int rtnVal = popMatch.Success && popMatch.Groups[1] != null
                ? int.Parse(popMatch.Groups[1].Value, NumberStyles.AllowThousands) : 0;
            return rtnVal;
        }
    }
}