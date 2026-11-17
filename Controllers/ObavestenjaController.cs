using Microsoft.AspNetCore.Mvc;
using PrijavaIspita.Models.EFRepository;
using X.PagedList; // PagedList
using System.Linq;
using X.PagedList.Extensions;

public class ObavestenjaController : Controller
{
    private readonly PrijavaIspitaRepository _repo;

    public ObavestenjaController()
    {
        _repo = new PrijavaIspitaRepository();
    }

    public IActionResult Index(int? page)
    {
        int? studentId = HttpContext.Session.GetInt32("KorisnikID");
        if (studentId == null)
            return RedirectToAction("Login", "Account");

        var obavestenja = _repo.GetObavestenja(studentId.Value)
                               .OrderByDescending(o => o.Datum)
                               .ToList();

        int pageSize = 10;
        int pageNumber = page ?? 1;

        // Kreiramo paged listu
        IPagedList<PrijavaIspita.Models.ObavestenjeBO> pagedObavestenja =
            obavestenja.ToPagedList(pageNumber, pageSize);

        return View(pagedObavestenja);
    }
}
