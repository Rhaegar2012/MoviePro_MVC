using MoviePro_MVC5._0.Models.Database;
using MoviePro_MVC5._0.Models.TMDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace MoviePro_MVC5._0.Services.Interfaces
{
    //Service for data transfer, maps Actor and Movie data
    public interface IDataMappingService
    {
        Task<Movie> MapMovieDetailAsync(MovieDetail movie);
        ActorDetail MapActorDetail(ActorDetail actor);
    }
}
