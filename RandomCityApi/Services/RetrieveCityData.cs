using RandomCityApi.Models;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RandomCityApi.Services;

namespace RandomCityApi.Services
{
    public class CityData
    {
        public int population;
        public bool wikiPop;
        public decimal longitude;
        public decimal latitude;
        public string summary;
        public string wikiRef;
        public int area;
    }
    public class RetrieveCityData
    {
        private WebClient client = new WebClient();
        private GetWikiData getWikiData = new GetWikiData();

        public async Task<CityData> GetCityData(City city) {
            CityData returnData = new CityData();

            // Pull out just country code from country
            var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures); 
            var code = regions.FirstOrDefault(r => r.EnglishName.Contains(city.Country)).Name;
            code = code.Substring(code.LastIndexOf('-') + 1);

            // Request city data from opendatasoft
            var uri = $"https://public.opendatasoft.com/api/records/1.0/search/?dataset=worldcitiespop&q={city.Name},%20{code}&sort=population&facet=country";
            string data = await client.DownloadStringTaskAsync(uri);
            dynamic cityData = JsonConvert.DeserializeObject(data);

            // Assign data and return
            if (cityData.nhits > 0)
            {
                // Potential exception here if hits with no records
                // is that possible?
                var dataFields = cityData.records[0].fields;
                returnData.population = dataFields.population != null
                    ? (int)dataFields.population : 0;
                returnData.latitude = dataFields.latitude != null 
                    ? (decimal)dataFields.latitude : 0;
                returnData.longitude = dataFields.longitude != null
                    ? (decimal)dataFields.longitude : 0;
                
                // Sometimes we dont get a population, try to find that info on wikipedia
                if (returnData.population == 0)
                {
                    var popData = await getWikiData.GetPopulationFallback(city);
                    if (popData != null && popData.population > 0) {
                        returnData.population = popData.population;
                        returnData.wikiRef = popData.wikiRef;
                        returnData.wikiPop = true;
                    }
                }
                else
                {
                    returnData.wikiPop = false;
                }
            }
            return returnData;
        }
    }
}