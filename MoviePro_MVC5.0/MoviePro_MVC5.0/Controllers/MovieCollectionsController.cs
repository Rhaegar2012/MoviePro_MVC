using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviePro_MVC5._0.Data;
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
        public async Task <IActionResult> Index(int?id)
        {
            id ??= (await _context.Collection.FirstOrDefaultAsync(c => c.Name.ToUpper() == "ALL")).Id;
            //Displays selection list for collection 
            ViewData["CollectionId"] = new SelectList(_context.Collection, "id", "Name", id);
            var allMovieIds = await _context.Movie.Select(m => m.Id).ToListAsync();
            var movieIdsCollection = await _context.MovieCollection
                                                   .Where(m => m.CollectionId == id)
                                                   .OrderBy(m => m.Order)
                                                   .Select(m => m.MovieId)
                                                   .ToListAsync();
            return View();
        }
    }
}
