using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviePro_MVC5._0.Models.Database
{
    public class MovieCast
    {
        //Primary key 
        public int Id { get; set; }
        //Foreign key
        public int MovieId { get; set;}
        public int CastID { get; set; }
        public string Department { get; set; }
        public string Name { get; set; }
        public string Character { get; set; }
        public string ImageUrl { get; set; }
        //Navigational properties
        //Parent class
        public Movie Movie { get; set; }


    }
}
