using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using RandomCity.Models;
using RandomCity.Services;

namespace RandomCity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomCityController : ControllerBase
    {
        private readonly CityContext _context;
        private RetrieveCityData getData = new RetrieveCityData();
        public RandomCityController(CityContext context)
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
                await _context.SaveChangesAsync();
            }
            return city;
        }

        // GET api/randomcity
        // Add an ID endpoint, that way you can test the DB access
        // functionality, and make sure you're not calling GetPopulation
        [HttpGet]
        public async Task<ActionResult<City>> GetRandomCity()
        {
            var selectId = new Random().Next(1, _context.Cities.Count());
            return await this.GetCityInfo(selectId);
        }

        // GET api/randomcity/501
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetRandomCityId(int id)
        {
            return await this.GetCityInfo(id);
        }
    }
}