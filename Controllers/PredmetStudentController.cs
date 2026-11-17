using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrijavaIspita.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace PrijavaIspita.Controllers
{
    public class PredmetStudentController : Controller
    {
        private readonly PrijavaIspitaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PredmetStudentController(PrijavaIspitaContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index(string filter = "svi")
        {
            var email = _httpContextAccessor.HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var student = _context.Korisnici
                .Include(s => s.Smer)
                .FirstOrDefault(s => s.Email == email);

            if (student == null)
                return NotFound("Student nije pronađen.");

            var predmeti = _context.Predmet
                .Include(p => p.Profesor)
                .Include(p => p.Smer)
                .Where(p => p.SmerId == student.SmerId)
                .ToList();

            // Dohvati sve prijave studenta da proverimo ocene
            var prijave = _context.Prijavas
                .Where(p => p.KorisnikID == student.KorisnikID)
                .ToList();

            // Filtriranje
            if (filter == "polozeni")
            {
                predmeti = predmeti.Where(p =>
                    prijave.Any(pr => pr.PredmetId == p.PredmetId && pr.Ocena > 5)
                ).ToList();
            }
            else if (filter == "nepolozeni")
            {
                predmeti = predmeti.Where(p =>
                    !prijave.Any(pr => pr.PredmetId == p.PredmetId && pr.Ocena > 5)
                ).ToList();
            }

            ViewBag.Filter = filter;

            return View(predmeti);
        }

        public IActionResult Details(int id)
        {
            var predmet = _context.Predmet
                .Include(p => p.Profesor)
                .Include(p => p.Smer)
                .FirstOrDefault(p => p.PredmetId == id);

            if (predmet == null)
                return NotFound();

            return View(predmet);
        }
    }
}
