using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrijavaIspita.Models;
using System.Linq;

namespace PrijavaIspita.Controllers
{
    [Authorize(Roles = "Profesor")]
    public class ProfesorOceneController : Controller
    {
        private readonly PrijavaIspitaContext _context;

        public ProfesorOceneController(PrijavaIspitaContext context)
        {
            _context = context;
        }

        // LISTA PRIJAVA SA FILTROM PO PREDMETU I STATUSU OCENE
        public IActionResult Index(int? predmetId, string statusOcene)
        {
            var profesorId = int.Parse(User.FindFirst("KorisnikID").Value);

            // filtriramo samo predmete koje predaje profesor
            var predmetiProfesora = _context.Predmet
                .Where(p => p.ProfesorID == profesorId)
                .Select(p => new { p.PredmetId, p.Naziv })
                .ToList();

            ViewBag.PredmetiProfesora = new SelectList(predmetiProfesora, "PredmetId", "Naziv", predmetId);

            // Status ocene dropdown
            var statusOpcije = new[]
            {
                new SelectListItem { Value = "", Text = "Svi studenti" },
                new SelectListItem { Value = "Nepolozeni", Text = "Nepoloženi studenti" },
                new SelectListItem { Value = "Polozeni", Text = "Položeni studenti" }
            };
            ViewBag.StatusOcene = new SelectList(statusOpcije, "Value", "Text", statusOcene);

            // filtriramo prijave po predmetu i statusu ocene
            var prijaveQuery = _context.Prijavas
                .Include(p => p.Korisnik)
                .Include(p => p.Predmet)
                .Where(p => p.Predmet.ProfesorID == profesorId);

            if (predmetId.HasValue)
            {
                prijaveQuery = prijaveQuery.Where(p => p.PredmetId == predmetId.Value);
            }

            if (!string.IsNullOrEmpty(statusOcene))
            {
                if (statusOcene == "Nepolozeni")
                    prijaveQuery = prijaveQuery.Where(p => p.Ocena == null || p.Ocena <= 5);
                else if (statusOcene == "Polozeni")
                    prijaveQuery = prijaveQuery.Where(p => p.Ocena > 5);
            }

            var prijave = prijaveQuery.ToList();

            return View(prijave);
        }

        // GET - UNOS OCENE
        public IActionResult UnosOcene(int predmetId, int korisnikId, string rok)
        {
            var prijava = _context.Prijavas
                .Include(p => p.Korisnik)
                .Include(p => p.Predmet)
                .FirstOrDefault(p =>
                    p.PredmetId == predmetId &&
                    p.KorisnikID == korisnikId &&
                    p.Rok == rok);

            if (prijava == null)
                return NotFound();

            return View(prijava);
        }

        // POST - UNOS OCENE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UnosOcene(int predmetId, int korisnikId, string rok, int ocena)
        {
            var prijava = _context.Prijavas
                .Include(p => p.Predmet)
                .FirstOrDefault(p =>
                    p.PredmetId == predmetId &&
                    p.KorisnikID == korisnikId &&
                    p.Rok == rok);

            if (prijava == null)
            {
                TempData["Greska"] = "Prijava nije pronađena.";
                return RedirectToAction("Index");
            }

            try
            {
                prijava.Ocena = ocena;
                _context.SaveChanges();

                _context.Obavestenja.Add(new Obavestenje
                {
                    KorisnikID = korisnikId,
                    Tekst = $"Ocena iz predmeta {prijava.Predmet.Naziv} Vam je uneta!",
                    Datum = DateTime.Now
                });
                _context.SaveChanges();

                TempData["Uspesno"] = "Uspešno uneta ocena.";
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greška pri unosu ocene: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
