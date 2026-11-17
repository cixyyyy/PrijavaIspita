using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrijavaIspita.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace PrijavaIspita.Controllers
{
    public class IspitTerminController : Controller
    {
        private readonly PrijavaIspitaContext _context;

        public IspitTerminController(PrijavaIspitaContext context)
        {
            _context = context;
        }

        // STUDENT - pregled kalendara ispita
        public IActionResult Index()
        {
            int korisnikId = int.Parse(User.FindFirst("KorisnikID").Value);

            var student = _context.Korisnici
                .Include(k => k.Smer)
                .FirstOrDefault(k => k.KorisnikID == korisnikId);

            if (student == null)
                return RedirectToAction("Login", "Account");

            var ispiti = _context.IspitTermini
                .Include(i => i.Predmet)
                .Where(i => i.Predmet.SmerId == student.Smer.SmerId)
                .ToList();

            return View(ispiti);
        }

        // PROFESOR - forma za zakazivanje
        [Authorize(Roles = "Profesor")]
        public IActionResult Zakazi()
        {
            int profesorId = int.Parse(User.FindFirst("KorisnikID").Value);

            var predmeti = _context.Predmet
                .Where(p => p.ProfesorID == profesorId)
                .ToList();

            ViewBag.Predmeti = predmeti;

            return View();
        }

        // PROFESOR - čuvanje termina
        [HttpPost]
        [Authorize(Roles = "Profesor")]
        public IActionResult Zakazi(int predmetId, string rok, DateTime datum)
        {
            // PROVERA
            var postoji = _context.IspitTermini
                .Any(i => i.PredmetId == predmetId && i.Rok == rok);

            if (postoji)
            {
                TempData["Greska"] = "Ispit za ovaj predmet je već zakazan u ovom roku!";
                return RedirectToAction("Zakazi");
            }

            // Ako ne postoji zakazan ispit u istom roku - Pravi novi termin
            var termin = new IspitTermin
            {
                PredmetId = predmetId,
                Rok = rok,
                Datum = datum
            };

            _context.IspitTermini.Add(termin);
            _context.SaveChanges();

            TempData["Uspesno"] = "Uspešno zakazan ispit!";

            return RedirectToAction("Zakazi");
        }
    }
}