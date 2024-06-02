using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Race.Models;

namespace Race.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        HttpContext.Session.Remove("sessionId");

        if(HttpContext.Session.GetString("adminId") != null)
        {
            Etape e = new Etape();
            Data data = new Data();
            
            Connexion coco = new Connexion();
            coco.connection.Open();

            data.etapeList = e.findAll(coco);

            coco.connection.Close();

            return View("listeEtape", data);

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult createEtape()
    {
        HttpContext.Session.Remove("sessionId");

        if(HttpContext.Session.GetString("adminId") != null)
        {
            var nom = Request.Form["nom"].ToString();
            double lkm = (double)Convert.ToInt32(Request.Form["lkm"]);
            int nbCoureur = Convert.ToInt32(Request.Form["nbCoureur"]);
            var rangEtape = Request.Form["rangEtape"].ToString();

            Etape insert = new Etape(nom, lkm, nbCoureur, rangEtape);
            
            Connexion coco = new Connexion();
            coco.connection.Open();

            insert.create(coco);

            coco.connection.Close();

            return RedirectToAction("Index", "Admin");

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult affecterTemps(string idEtape)
    {
        HttpContext.Session.Remove("sessionId");

        if(HttpContext.Session.GetString("adminId") != null)
        {
            Data data = new Data();
            
            Connexion coco = new Connexion();
            coco.connection.Open();

            data.etape = new Etape().findById(coco, idEtape);
            data.coureurList = new Coureur().etapeCoureur(coco, idEtape);

            coco.connection.Close();

            if (TempData["ErrorAffecte"] != null)
            {
                ViewBag.Error = TempData["ErrorAffecte"];
            }

            return View("AffecterTemps", data);

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult affecte()
    {
        HttpContext.Session.Remove("sessionId");

        if(HttpContext.Session.GetString("adminId") != null)
        {
            var idEtape = Request.Form["idEtape"].ToString();

            var idEtapeCoureurList = Request.Form["idEtapeCoureur"].ToList();
            var hDepartList = Request.Form["hDepart"].ToList();
            var hArriverList = Request.Form["hArriver"].ToList();

            if (idEtapeCoureurList.Count != hDepartList.Count && hDepartList.Count != hArriverList.Count)
            {
                TempData["ErrorAffecte"] = "Tsy mitovy ny isanl le coureur sy ny time voaray";
                return RedirectToAction("affecterTemps", "Admin", new { idEtape = idEtape });
            }else
            {
                Connexion coco = new Connexion();
                coco.connection.Open();

                for (int i = 0; i < idEtapeCoureurList.Count; i++)
                {
                    TimeSpan hDepart = TimeSpan.Parse(hDepartList[i]);
                    TimeSpan hArriver = TimeSpan.Parse(hArriverList[i]);
                    Console.WriteLine(hDepart);
                    Console.WriteLine(hArriver);
                    new CoureurTemps(idEtapeCoureurList[i], hDepart, hArriver).create(coco);
                }
                
                coco.connection.Close();

            }
            return RedirectToAction("Index", "Admin");

        }else{

            return RedirectToAction("Index", "Home");
        }
    }
}
