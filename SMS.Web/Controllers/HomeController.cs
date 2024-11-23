using Microsoft.AspNetCore.Mvc;

namespace SMS.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
