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
        public RandomCityApiController(CityContext context)
        {
            _context = context;
        }

        private async Task<ActionResult<City>> GetCityInfo(int id)
        {
            var city = await _context.Cities.FindAsync(id);
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
            return city;
        }

        // GET api/RandomCityApi
        // Add an ID endpoint, that way you can test the DB access
        // functionality, and make sure you're not calling GetPopulation
        [HttpGet]
        public async Task<ActionResult<City>> GetRandomCityApi()
        {
            var selectId = new Random().Next(1, _context.Cities.Count());
            return await this.GetCityInfo(selectId);
        }

        // GET api/RandomCityApi/501
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetRandomCityApiId(int id)
        {
            return await this.GetCityInfo(id);
        }
    }
}