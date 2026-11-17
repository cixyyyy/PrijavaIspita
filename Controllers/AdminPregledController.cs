using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrijavaIspita.Models;
using System.Linq;
using X.PagedList;
using X.PagedList.Extensions;

namespace PrijavaIspita.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminPregledController : Controller
    {
        private readonly PrijavaIspitaContext _context;

        public AdminPregledController()
        {
            _context = new PrijavaIspitaContext();
        }

        public IActionResult Index()
        {
            return View();
        }
        //Ispis liste prijava za admina
        public IActionResult PrijaveAdmin(int? page)
        {
            int pageSize = 10; // PAGINACIJA
            int pageNumber = page ?? 1;

            var prijave = _context.Prijavas
                .Select(p => new PrijavaIspitaBO
                {
                    Id = p.PredmetId,
                    Ocena = p.Ocena,
                    Rok = p.Rok,
                    DatumPrijave = p.DatumPrijave,
                    Predmet = new PredmetBO
                    {
                        Id = p.Predmet.PredmetId,
                        Naziv = p.Predmet.Naziv,
                        ProfesorID = (int)p.Predmet.ProfesorID,
                        ProfesorIme = p.Predmet.Profesor.Ime,
                        ProfesorPrezime = p.Predmet.Profesor.Prezime
                    },
                    Korisnik = new KorisnikBO
                    {
                        Id = p.Korisnik.KorisnikID,
                        Ime = p.Korisnik.Ime,
                        Prezime = p.Korisnik.Prezime,
                        Email = p.Korisnik.Email
                    }
                })
                .OrderByDescending(p => p.DatumPrijave)
                .ToPagedList(pageNumber, pageSize);

            return View(prijave);
        }


        //Ispis transakcija za admina
        public IActionResult TransakcijeAdmin(int? page)
        {
            int pageSize = 10; // PAGINACIJA - broj stavki po stranici
            int pageNumber = page ?? 1;

            var transakcije = _context.Transakcijas
                .Select(t => new TransakcijaBO
                {
                    Id = t.TransakcijaId,
                    Tip = t.Tip,
                    Iznos = (float)t.Iznos,
                    Datum = t.Datum,
                    Korisnik = new KorisnikBO
                    {
                        Id = t.Korisnik.KorisnikID,
                        Ime = t.Korisnik.Ime,
                        Prezime = t.Korisnik.Prezime,
                        Email = t.Korisnik.Email
                    }
                })
                .OrderByDescending(t => t.Datum) // sortiranje po datumu, najnovije prve
                .ToPagedList(pageNumber, pageSize);

            return View(transakcije);
        }

    }
}
