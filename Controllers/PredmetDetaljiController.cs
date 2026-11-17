using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrijavaIspita.Models;

namespace PrijavaIspita.Controllers
{
    public class PredmetDetaljiController : Controller
    {
        private readonly PrijavaIspitaContext _context;

        public PredmetDetaljiController(PrijavaIspitaContext context)
        {
            _context = context;
        }

        // GET
        public IActionResult Detaljnije(int id)
        {
            // Dohvati ID ulogovanog studenta iz session-a
            var studentId = HttpContext.Session.GetInt32("KorisnikID");

            if (studentId == null)
                return RedirectToAction("Login", "Account"); // ili NotFound()

            // Učitaj studenta sa smerom
            var student = _context.Korisnici
                .Include(s => s.Smer)
                .FirstOrDefault(s => s.KorisnikID == studentId.Value);

            if (student == null)
                return NotFound("Student ne postoji.");

            // Učitaj predmet samo ako je za njegov smer
            var predmet = _context.Predmet
                .Include(p => p.Profesor)
                .Include(p => p.Smer)
                .FirstOrDefault(p => p.PredmetId == id && p.SmerId == student.SmerId);

            if (predmet == null)
                return NotFound("Predmet nije dostupan za vaš smer.");

            return View(predmet);
        }
    }
}
