using Microsoft.AspNetCore.Mvc;

namespace MoviePro_MVC5._0.Controllers
{
    public class CollectionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
