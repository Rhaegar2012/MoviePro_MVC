using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        //Bring movies from database as a list 
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
        //Library action import all the imported movies outside of the respective movie collections
        public async Task<IActionResult> Library()
        {
            var movies = await _context.Movie.ToListAsync();
            return View(movies);
        }
        //GET :Temp/Create
        //
        public IActionResult Create()
        {
            //Supplies data as a dropdown list
            ViewData["CollectionId"] = new SelectList(_context.Collection, "Id", "Name");
            return View();

        }
        //POST: Tempt/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieId,Title,TagLine,Overview,Runtime,ReleaseDate,Rating,VoteAverage,Poster,PosterType,Backdrop,BackropType,TrailerUrl")] Movie movie,int collectionId)
        {
            if (ModelState.IsValid)
            {
                //Captures poster type and data from incoming form
                movie.PosterType = movie.PosterFile?.ContentType;
                movie.Poster = await _imageService.EncodeImageAsync(movie.PosterFile);
                //Captures backdrop type and data from incoming form
                movie.BackdropType = movie.BackdropFile?.ContentType;
                movie.Backdrop = await _imageService.EncodeImageAsync(movie.BackdropFile);
                _context.Add(movie);
                await _context.SaveChangesAsync();
                //Adds movie Id to the incoming collectionId 
                await AddToMovieCollection(movie.Id, collectionId);
                return RedirectToAction("Index", "MovieCollections");

            }
            return View(movie);
        }
        //GET : Temp/Edit/5
        //Requests a movie to be editable by ID if not found returns null
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        //POST: Temp/Edit/5
        //Uploads changes made to a move element to the databaset 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id , [Bind("Id,MovieId,Title,Tagline,Overview,Runtime,ReleaseDate,Rating,VoteAverage,Poster,PosterType,Backdrop,BackdropType,TrailerUrl")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //If PosterFile or BackdropFile are updated
                try
                {
                    if(movie.PosterFile is not null)
                    {
                        movie.PosterType = movie.PosterFile.ContentType;
                        movie.Poster = await _imageService.EncodeImageAsync(movie.PosterFile);
                    }
                    if(movie.BackdropFile is not null)
                    {
                        movie.BackdropType = movie.BackdropFile.ContentType;
                        movie.Backdrop = await _imageService.EncodeImageAsync(movie.BackdropFile);
                    }
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Movies", new { id = movie.Id, local = true });
            }
            return View(movie);
        }

        //GET: Temp/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        //POST: Temp/Delete/5
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction("Library", "Movies");
        }
        //Checks if the movie exists in database
        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
        //GET: DETAILS
        //Goes to details page of a movie
        public async Task<IActionResult> Details(int?id,bool local = false)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Movie to be rendered in view 
            Movie movie = new();
            if (local)
            {
                //Get the movie data straight from the DB
                movie = await _context.Movie.Include(m => m.Cast)
                                          .Include(m => m.Crew)
                                          .Include(m=> m.Poster)
                                          .Include(m=>m.PosterType)
                                          .FirstOrDefaultAsync(m => m.Id == id);
            }
            else
            {
                //Get the movie data from the TMDB API 
                var movieDetail = await _tmdbMovieService.MovieDetailAsync((int)id);
                movie = await _tmdbMappingService.MapMovieDetailAsync(movieDetail);
            }
            //If movie is not found neither on database or TMDB API 
            if(movie == null)
            {
                return NotFound();
            }
            ViewData["Local"] = local;
            return View(movie);

        }

    }
}
