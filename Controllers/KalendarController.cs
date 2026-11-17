using Microsoft.AspNetCore.Mvc;

namespace PrijavaIspita.Controllers
{
    public class KalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
