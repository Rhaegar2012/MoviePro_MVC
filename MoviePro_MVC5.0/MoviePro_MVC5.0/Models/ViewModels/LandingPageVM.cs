using MoviePro_MVC5._0.Models.Database;
using MoviePro_MVC5._0.Models.TMDB;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace MoviePro_MVC5._0.Models.ViewModels   
{
    public class LandingPageVM
    {
        public List<Collection> CustomCollections { get; set;}
        public MovieSearch NowPlaying { get; set; }
        public MovieSearch Popular { get; set; }
        public MovieSearch TopRated { get; set; }
        public MovieSearch Upcoming { get; set; }

    }
}
