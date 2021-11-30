using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoviePro_MVC5._0.Data;
using MoviePro_MVC5._0.Models.Database;
using MoviePro_MVC5._0.Models.Settings;
using MoviePro_MVC5._0.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace MoviePro_MVC5._0.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IRemoteMovieService _tmdbMovieService;
        private readonly IDataMappingService _tmdbMappingService;


        public MoviesController(IOptions<AppSettings> appSettings, ApplicationDbContext context, IImageService imageService, IRemoteMovieService tmdbMovieService, IDataMappingService tmdbMappingService)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _imageService = imageService;
            _tmdbMovieService = tmdbMovieService;
            _tmdbMappingService = tmdbMappingService;
        }

        public async Task<IActionResult> Import()
        {
            var movies = await _context.Movie.ToListAsync();
            return View(movies);
        }
        //Recieves movie request from user and requests it to the API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(int id)
        {
            //Prevent duplicate movie imports
            if (_context.Movie.Any(m => m.MovieId == id))
            {
                var localMovie = await _context.Movie.FirstOrDefaultAsync(m => m.MovieId == id);
                //Redirect to Action Details in the Movies controller
                return RedirectToAction("Details", "Movies", new { id = localMovie.Id, local = true });
            }
            //Step 1: Get the raw data from the API 
            var movieDetail = await _tmdbMovieService.MovieDetailAsync(id);
            //Step 2: Run the data through a mapping procedure 
            var movie = await _tmdbMappingService.MapMovieDetailAsync(movieDetail);
            //Step 3: Add the new movie 
            _context.Add(movie);
            await _context.SaveChangesAsync();
            //Step 4: Assign it to the default All Collection 
            await AddToMovieCollection(movie.Id, _appSettings.MovieProSettings.DefaultCollection.Name);
            return RedirectToAction("Import");
        }



        //First overload of AddToMovieCollection 
        private async Task AddToMovieCollection(int movieId, string collectionName)
        {
            var collection = await _context.Collection.FirstOrDefaultAsync(c => c.Name == collectionName);
            _context.Add(
                new MovieCollection()
                {
                    CollectionId = collection.Id,
                    MovieId = movieId,
                }
               );
            await _context.SaveChangesAsync();
        }
        //Second overload of AddToMovieCollection 
        private async Task AddToMovieCollection(int movieId, int collectionId)
        {
            _context.Add(
                new MovieCollection()
                {
                    CollectionId = collectionId,
                    MovieId = movieId
                }
                );
            await _context.SaveChangesAsync();
        }
    }
}
