using MoviePro_MVC5._0.Enums;
using MoviePro_MVC5._0.Models.TMDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace MoviePro_MVC5._0.Services.Interfaces
{
    //Service to point to alternative API's for movie database
    public interface IRemoteMovieService
    {
        Task<MovieDetail> MovieDetailAsync(int id);
        Task<MovieSearch> SearchMoviesAsync(MovieCategory category, int count);
        Task<ActorDetail> ActorDetailAsync(int id);
    }
}
