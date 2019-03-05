namespace RandomCity.Models
{
    public class City
    {
        // Should include here a field for a summary, a link
        // to the wikipedia page (if used), and an indication
        // if population data was from wikipedia
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Subcountry { get; set; }
        public int? Population { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
    }
}