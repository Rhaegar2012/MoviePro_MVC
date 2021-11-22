namespace MoviePro_MVC5._0.Models.Database
{
    public class MovieCrew
    {
        //Primary key 
        public int Id { get; set; }
        //Foreign key
        public int MovieId { get; set; }
        public int CrewID { get; set; }
        public string Department { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public string ImageUrl  { get; set; }

        //Navigational properties
        //Parent Class 
        public Movie Movie  { get; set; }

    }
}
