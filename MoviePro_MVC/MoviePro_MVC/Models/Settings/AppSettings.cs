using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace MoviePro_MVC.Models.Settings
{
    public class AppSettings
    {
        //Settings model to configure the Movie Pro API property
        
            public MovieProSettings MovieProSettings { get; set; }
            public TMDBSettings TMBDSettings { get; set; }
        
    }
}
