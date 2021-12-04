namespace MoviePro_MVC5._0.Models.Database
{
    public class MovieCollection
    {
        public int id { get; set; }
        public int CollectionId { get; set; }
        public int MovieId { get; set; }
        public int Order { get; set; }
        //Navigational properties
        public Collection Collection { get; set; }
        //Parent property 
        public Movie Movie { get; set; }
    }
}
