using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrijavaIspita.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PrijavaIspita.Controllers
{
    public class AccountController : Controller
    {
        private readonly PrijavaIspitaContext _context;

        public AccountController(PrijavaIspitaContext context)
        {
            _context = context;
        }

        // GET: Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var korisnik = await _context.Korisnici
                .FirstOrDefaultAsync(k => k.Email == model.Email && k.Password == model.Password);

            if (korisnik == null)
            {
                ModelState.AddModelError("", "Neispravan email ili lozinka.");
                return View(model);
            }

            // uzimamo rolu korisnika
            var userRole = await _context.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.KorisnikID == korisnik.KorisnikID);

            string roleName = userRole != null ? userRole.Role.RoleName : "Student";

            // KREIRANJE CLAIMSa
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, korisnik.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("KorisnikID", korisnik.KorisnikID.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // SIGN-IN
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // SESIJA
            HttpContext.Session.SetString("Email", korisnik.Email);
            HttpContext.Session.SetString("Role", roleName);
            HttpContext.Session.SetInt32("KorisnikID", korisnik.KorisnikID);

            return RedirectToAction("Index", "Home");
        }

        // GET: Register
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Smerovi = _context.Smer.ToList();
            return View();
        }

        // POST: Register
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Smerovi = _context.Smer.ToList();
                return View(model);
            }

            var korisnik = new Korisnik
            {
                Ime = model.Ime,
                Prezime = model.Prezime,
                Email = model.Email,
                Password = model.Password,
                SmerId = model.SmerId,
                DatumRodjenja = model.DatumRodjenja,
                GodinaUpisa = model.GodinaUpisa
            };

            // Dodaj korisnika u bazu
            _context.Korisnici.Add(korisnik);
            await _context.SaveChangesAsync();

            // Dodaj UserRole automatski (Student)
            _context.UserRoles.Add(new UserRole
            {
                KorisnikID = korisnik.KorisnikID,
                RoleID = 1 // Student
            });
            await _context.SaveChangesAsync();

            // Dodaj budžet za novog korisnika
            _context.Budzets.Add(new Budzet
            {
                KorisnikID = korisnik.KorisnikID,
                Iznos = 1000,
                Status = "Aktivan"
            });
            await _context.SaveChangesAsync();

            // KREIRANJE CLAIMS
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, korisnik.Email),
                new Claim(ClaimTypes.Role, "Student"),
                new Claim("KorisnikID", korisnik.KorisnikID.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // SIGN-IN
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // SESIJA
            HttpContext.Session.SetString("Email", korisnik.Email);
            HttpContext.Session.SetString("Role", "Student");
            HttpContext.Session.SetInt32("KorisnikID", korisnik.KorisnikID);

            return RedirectToAction("Index", "Home");
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear(); // briši sve sesije prilikom LOGOUT-a

            return RedirectToAction("Login");
        }
    }
}
