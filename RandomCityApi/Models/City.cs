namespace RandomCityApi.Models
{
    public class City
    {
        // Should  a link to the wikipedia page (if used), and an indication
        // if  data was retrieved from wikipedia
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Subcountry { get; set; }
        public int? Population { get; set; }
        public bool? WikiPop { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public string Summary { get; set; }
        public int? Area { get; set; }
        public string WikiRef { get; set; }
    }
}