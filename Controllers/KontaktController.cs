using Microsoft.AspNetCore.Mvc;
using PrijavaIspita.Models;
using System.Collections.Generic;

namespace PrijavaIspita.Controllers
{
    public class KontaktController : Controller
    {
        // Privremena lista za čuvanje poruka u memoriji
        private static List<KontaktPoruka> poruke = new List<KontaktPoruka>();

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PosaljiPoruku(KontaktPoruka poruka)
        {
            if (!ModelState.IsValid)
            {
                // Ako validacija ne uspe, vrati formu sa trenutnim unosom
                return View("Index", poruka);
            }

            // Dodaj poruku u privremenu listu (u memoriji)
            poruke.Add(poruka);

            // Feedback korisniku
            TempData["Uspesno"] = "Poruka je uspešno poslata!";

            return RedirectToAction("Index");
        }
    }
}
