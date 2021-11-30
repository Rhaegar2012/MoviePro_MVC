using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviePro_MVC5._0.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MoviePro_MVC5._0.Data;
using MoviePro_MVC5._0.Services;
using MoviePro_MVC5._0.Services.Interfaces;
using MoviePro_MVC5._0.Models.ViewModels;
using MoviePro_MVC5._0.Enums;

namespace MoviePro_MVC5._0.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRemoteMovieService _tmdbMovieService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IRemoteMovieService tmdbMovieService)
        {
            _logger = logger;
            _context = context;
            _tmdbMovieService = tmdbMovieService;
        }

        public async Task<IActionResult> Index()

        {
            const int count = 16;
            //Data that goes into the view 
            var data = new LandingPageVM()
            {
                //Retrieves collections records to populate landing page by connecting with TMDB Movie Service
                CustomCollections = await _context.Collection
                                             .Include(c => c.MovieCollection)
                                             .ThenInclude(mc => mc.Movie)
                                             .ToListAsync(),
                NowPlaying = await _tmdbMovieService.SearchMoviesAsync(MovieCategory.now_playing, count),
                Popular= await _tmdbMovieService.SearchMoviesAsync(MovieCategory.popular,count),
                TopRated = await _tmdbMovieService.SearchMoviesAsync(MovieCategory.top_rated,count),
                Upcoming = await _tmdbMovieService.SearchMoviesAsync(MovieCategory.upcoming,count)

            };
            //Pushes data object into the view 
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
