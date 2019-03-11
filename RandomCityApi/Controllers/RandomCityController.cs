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
                // These don't necessarily need to come back on this call
                city.Summary = cityData.summary;
                city.Area = cityData.area;
                await _context.SaveChangesAsync();
            }
        }

        public async Task GetSummaryData(City city)
        {
            if (city.Summary == null)
            {
                var sumData = await getWikiData.GetWikiCityData(city);
                city.Summary = sumData.summary;
                city.Area = sumData.area;
                await _context.SaveChangesAsync();
            }
        }

        // GET api/RandomCityApi
        // Add an ID endpoint, that way you can test the DB access
        // functionality, and make sure you're not calling GetPopulation
        [HttpGet]
        public async Task<ActionResult<City>> GetRandomCity()
        {
            var selectId = new Random().Next(1, _context.Cities.Count());
            var city = await _context.Cities.FindAsync(selectId);
            var tasks = new List<Task>();
            tasks.Add(Task.Run(() => this.GetCityInfo(city)));
            tasks.Add(Task.Run(() => this.GetSummaryData(city)));
            await Task.WhenAll(tasks);
            return city;
        }

        // GET api/RandomCityApi/501
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetRandomCityId(int id)
        {
            return await _context.Cities.FindAsync(id);
        }
    }
}