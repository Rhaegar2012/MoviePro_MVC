using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviePro_MVC5._0.Data;
using MoviePro_MVC5._0.Models.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviePro_MVC5._0.Controllers
{
    public class MovieCollectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieCollectionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //GET: MovieCollectionsIndex
        //Displays  selection lists with movies in movie collection and movies not in the movie collection of choice
        public async Task <IActionResult> Index(int?id)
        {
            //Checks if collection id is null , if it is null defaults it to All
            id ??= (await _context.Collection.FirstOrDefaultAsync(c => c.Name.ToUpper() == "ALL")).Id;
            //Displays selection list for collection 
            ViewData["CollectionId"] = new SelectList(_context.Collection, "id", "Name", id);
            var allMovieIds = await _context.Movie.Select(m => m.Id).ToListAsync();
            //Sellects all the movie ids that are present in the selected collection and returns a list
            var movieIdsCollection = await _context.MovieCollection
                                                   .Where(m => m.CollectionId == id)
                                                   .OrderBy(m => m.Order)
                                                   .Select(m => m.MovieId)
                                                   .ToListAsync();
            //Selects all ids not in the collection
            var movieIdsNotInCollection = allMovieIds.Except(movieIdsCollection);
            var moviesInCollection = new List<Movie>();
            movieIdsCollection.ForEach(movieId => moviesInCollection.Add(_context.Movie.Find(movieId)));
            ViewData["IdsInCollection"] = new SelectList(moviesInCollection, "Id", "Title");
            var moviesNotInCollection = await _context.Movie.AsNoTracking().Where(m => movieIdsNotInCollection.Contains(m.Id)).ToListAsync();
            ViewData["IdsNotInCollection"] = new SelectList(moviesNotInCollection, "Id", "Title");
            return View();
        }
        //POST: MovieCollectionIndex
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, List<int> idsInCollection)
        {
            //Removes all old records 
            var oldRecords = _context.MovieCollection.Where(c => c.CollectionId == id);
            _context.MovieCollection.RemoveRange(oldRecords);
            // Add new movies as new collection records 
            if (idsInCollection != null)
            {
                int index = 1;
                idsInCollection.ForEach(movieId => _context.Add(new MovieCollection()
                {
                    CollectionId = id,
                    MovieId = movieId,
                    Order = index++
                }));
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { id });

        }
    }
}
