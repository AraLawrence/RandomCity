using Microsoft.EntityFrameworkCore;

namespace RandomCity.Models
{
    public class CityContext : DbContext
    {
        public CityContext(DbContextOptions<CityContext> options)
            : base(options)
        { }

        public DbSet<City> Cities { get; set; }
    }
}