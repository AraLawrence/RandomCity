using Microsoft.EntityFrameworkCore;

namespace RandomCityApi.Models
{
    public class CityContext : DbContext
    {
        public CityContext(DbContextOptions<CityContext> options)
            : base(options)
        { }

        public DbSet<City> Cities { get; set; }
    }
}