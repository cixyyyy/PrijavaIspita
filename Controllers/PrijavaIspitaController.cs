using Microsoft.AspNetCore.Mvc;
using PrijavaIspita.Models;
using PrijavaIspita.Models.EFRepository;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System;

namespace PrijavaIspita.Controllers
{
    [Authorize(Roles = "Student")]
    public class PrijavaIspitaController : Controller
    {
        private readonly PrijavaIspitaRepository _prijavaIspitaRepository;

        public PrijavaIspitaController()
        {
            _prijavaIspitaRepository = new PrijavaIspitaRepository();
        }

        // GET - PrijavaIspita/Index
        public IActionResult Index()
        {
            int? korisnikId = HttpContext.Session.GetInt32("KorisnikID");
            if (korisnikId == null)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/PrijavaIspita/Index" });

            var korisnik = _prijavaIspitaRepository.GetAllKorisnici()
                .FirstOrDefault(s => s.Id == korisnikId.Value);

            if (korisnik == null)
                return RedirectToAction("Login", "Account");

            // Svi predmeti za njegov smer
            var predmetiSmera = _prijavaIspitaRepository.GetAllPredmeti()
                .Where(p => p.SmerId == korisnik.Smer.Id)
                .ToList();

            // Sve prijave korisnika
            var prijaveStudenta = _prijavaIspitaRepository.GetByKorisnikId(korisnik.Id);

            // Filtriraj predmete gde ocena nije >5
            var predmetiZaPrijavu = predmetiSmera
                .Where(p => !prijaveStudenta.Any(pr => pr.Predmet.Id == p.Id && pr.Ocena > 5))
                .ToList();

            ViewBag.Predmeti = predmetiZaPrijavu;
            ViewBag.Rokovi = _prijavaIspitaRepository.GetRokovi();
            ViewBag.StanjeBudzet = _prijavaIspitaRepository.GetBudzet(korisnik.Id);

            return View(prijaveStudenta);
        }

        // Filtriranje ispita po predmetu (AJAX ili partial view)
        public IActionResult GetIspiteByPredmet(int id)
        {
            int? korisnikId = HttpContext.Session.GetInt32("KorisnikID");
            if (korisnikId == null)
                return RedirectToAction("Login", "Account");

            var prijave = _prijavaIspitaRepository.GetByKorisnikId(korisnikId.Value);

            if (id != 0)
                prijave = prijave.Where(p => p.Predmet.Id == id).ToList();

            return PartialView("ListaIspita", prijave);
        }

        // POST - prijava predmeta
        [HttpPost]
        public IActionResult PrijaviPredmet(int PredmetId, string Rok)
        {
            int? korisnikId = HttpContext.Session.GetInt32("KorisnikID");
            if (korisnikId == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(Rok))
            {
                TempData["Greska"] = "Morate odabrati rok!";
                return RedirectToAction("Index");
            }

            var korisnik = _prijavaIspitaRepository.GetAllKorisnici()
                                .FirstOrDefault(s => s.Id == korisnikId.Value);

            var predmet = _prijavaIspitaRepository.GetAllPredmeti()
                                .FirstOrDefault(p => p.Id == PredmetId);

            if (korisnik == null || predmet == null)
            {
                TempData["Greska"] = "Korisnik ili predmet nisu pronađeni.";
                return RedirectToAction("Index");
            }

            try
            {
                // PROVERA DA LI JE VEĆ POLOŽIO
                bool postojiPrijava = _prijavaIspitaRepository.GetByKorisnikId(korisnik.Id)
                                        .Any(p => p.Predmet.Id == PredmetId && p.Ocena > 5);

                if (postojiPrijava)
                {
                    TempData["Greska"] = "Već ste položili ovaj predmet!";
                    return RedirectToAction("Index");
                }

                // KREIRANJE OBJEKTA
                var prijava = new PrijavaIspitaBO
                {
                    Korisnik = korisnik,
                    Predmet = predmet,
                    Rok = Rok,
                    DatumPrijave = DateTime.Now
                };

                // POZIV REPOSITORY
                _prijavaIspitaRepository.Add(prijava);

                // OBAVEŠTENJE
                _prijavaIspitaRepository.DodajObavestenje(
                    korisnik.Id,
                    $"Uspešno ste prijavili ispit: {predmet.Naziv}"
                );

                TempData["Uspesno"] = $"Uspešno ste prijavili ispit: {predmet.Naziv}";
            }
            catch (Exception ex)
            {
                TempData["Greska"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // Slanje zahteva za budzet
        [HttpPost]
        public IActionResult PosaljiZahtevBudzet(decimal kolicina)
        {
            int? korisnikId = HttpContext.Session.GetInt32("KorisnikID");
            if (korisnikId == null)
                return RedirectToAction("Login", "Account");

            _prijavaIspitaRepository.DodajZahtevBudzet(korisnikId.Value, kolicina);
            _prijavaIspitaRepository.DodajObavestenje(korisnikId.Value, $"Uspešno ste poslali zahtev za budzet u iznosu od: {kolicina} RSD");

            TempData["Uspesno"] = "Uspešno ste poslali zahtev za budžet!";
            return RedirectToAction("Index");
        }
    }
}
