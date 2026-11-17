using Microsoft.AspNetCore.Mvc;

namespace PrijavaIspita.Controllers
{
    public class DokumentacijaController : Controller
    {
        public IActionResult Cenovnik()
        {
            return View();
        }

        public IActionResult Uputstva()
        {
            return View();
        }

        public IActionResult Pravila()
        {
            return View();
        }
        public IActionResult Profesor()
        {
            return View();
        }
    }
}
