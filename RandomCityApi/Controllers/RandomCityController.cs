using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using RandomCityApi.Models;
using RandomCityApi.Services;

namespace RandomCityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomCityApiController : ControllerBase
    {
        private readonly CityContext _context;
        private RetrieveCityData getData = new RetrieveCityData();
        private GetWikiData getWikiData = new GetWikiData();
        public RandomCityApiController(CityContext context)
        {
            _context = context;
        }

        private async Task GetCityInfo(City city)
        {
            if (city.Population == 0 || city.Latitude == 0 || city.Longitude == 0)
            {
                var cityData = await getData.GetCityData(city);
                city.Population = cityData.population;
                city.Latitude = cityData.latitude;
                city.Longitude = cityData.longitude;
                city.WikiPop = cityData.wikiPop;
                await _context.SaveChangesAsync();
            }
        }

        public async Task GetSummaryData(City city)
        {
            if (city.Summary == null || city.Summary == String.Empty)
            {
                var sumData = await getWikiData.GetWikiCityData(city);
                city.Summary = sumData.summary;
                city.Area = sumData.area;
                city.WikiRef = sumData.wikiRef;
                await _context.SaveChangesAsync();
            }
        }

        // GET api/RandomCityApi
        // Add an ID endpoint, that way you can test the DB access
        // functionality, and make sure you're not calling GetPopulation
        [HttpGet]
        public async Task<ActionResult<City>> GetRandomCity()
        {
            int selectId = new Random().Next(1, _context.Cities.Count());
            City city = await _context.Cities.FindAsync(selectId);
            await this.GetCityInfo(city);
            return city;
        }

        // GET api/RandomCityApi/501
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetRandomCityId(int id)
        {
            City city = await _context.Cities.FindAsync(id);
            await this.GetSummaryData(city);
            return city;
        }
    }
}