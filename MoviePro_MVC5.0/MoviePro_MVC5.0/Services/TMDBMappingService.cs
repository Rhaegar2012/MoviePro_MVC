using Microsoft.Extensions.Options;
using MoviePro_MVC5._0.Enums;
using MoviePro_MVC5._0.Models.Database;
using MoviePro_MVC5._0.Models.Settings;
using MoviePro_MVC5._0.Models.TMDB;
using MoviePro_MVC5._0.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace MoviePro_MVC5._0.Services
{
    public class TMDBMappingService : IDataMappingService
    {
        private readonly AppSettings _appSettings;
        private readonly IImageService _imageService; 
        public TMDBMappingService(IOptions<AppSettings> appSettings, IImageService imageService)
        {
            _appSettings = appSettings.Value;
            _imageService = imageService;
        }

        public ActorDetail MapActorDetail(ActorDetail actor)
        {
            throw new NotImplementedException();
        }

        public async Task<Movie> MapMovieDetailAsync(MovieDetail movie)
        {
            Movie newMovie = null;
            try
            {
                newMovie = new Movie()
                {
                    MovieId = movie.id,
                    Title = movie.title,
                    TagLine = movie.tagline,
                    Overview = movie.overview,
                    RunTime = movie.runtime,
                    Backdrop = await EncodeBackdropImageAsync(movie.backdrop_path),
                    PosterType = BuildImageType(movie.backdrop_path),
                    Rating = GetRating(movie.release_dates),
                    ReleaseDate = DateTime.Parse(movie.release_date),
                    TrailerUrl = BuildTrailerPath(movie.videos),
                    VoteAverage = movie.vote_average
                };
                var castMembers = movie.credits.cast.OrderByDescending(c => c.popularity)
                                                    .GroupBy(c => c.cast_id)
            
                                                    .Select(g => g.FirstOrDefault())
                                                    .Take(20).ToList();
                castMembers.ForEach(member =>
                {
                newMovie.Crew.Add(new MovieCrew()
                    {
                        Id = member.id,
                        Department = member.known_for_department,
                        Name = member.name,
                        Job = member.character,
                        ImageUrl = BuildCastImage(member.profile_path),

                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in MapMovieDetailAsync: {ex.Message}");
            }
            return newMovie; 
        }
    }
}
