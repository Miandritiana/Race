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
            data.userList = new Uuser().findAll(coco);
            data.coureurList = new Coureur().findAll(coco);
            data.equipeList = new Uuser().allEquipes(coco);

            coco.connection.Close();

            return View("listeEtape", data);

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult ResetDatabase()
    {
        HttpContext.Session.Remove("sessionId");

        if(HttpContext.Session.GetString("adminId") != null)
        {
            Connexion coco = new Connexion();
            try{
                coco.connection.Open();
                    Connexion.ResetDatabase(coco);
                coco.connection.Close();
                ViewBag.Message = "Nety tsara";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            coco.connection.Close();
            return RedirectToAction("Index", "Admin", ViewBag);

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
            if (TempData["ErrorInsert"] != null)
            {
                ViewBag.Error = TempData["ErrorInsert"];
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
            TimeSpan hDepart = new();
            TimeSpan hArriver = new();

            var idEtape = Request.Form["idEtape"].ToString();

            var dhDepartString = Request.Form["dhDepart"].ToString();
            DateTime dhDepart = DateTime.Parse(dhDepartString);
            if (DateTime.TryParse(dhDepartString, out DateTime dateTime))
            {
                hDepart = dateTime.TimeOfDay;
            }

            var idEtapeCoureurList = Request.Form["idEtapeCoureur"].ToList();
            var dhArriverList = Request.Form["dhArriver"].ToList();

            if (idEtapeCoureurList.Count != dhArriverList.Count)
            {
                TempData["ErrorAffecte"] = "Tsy mitovy ny isan le coureur sy ny time voaray";
                return RedirectToAction("affecterTemps", "Admin", new { idEtape = idEtape });
            }else
            {
                try
                {
                    Connexion coco = new Connexion();
                    coco.connection.Open();

                    for (int i = 0; i < idEtapeCoureurList.Count; i++)
                    {
                        DateTime dhArriver = DateTime.Parse(dhArriverList[i]);
                        // TimeSpan hArriver = TimeSpan.Parse(dhArriverList[i].ToString());

                        if (DateTime.TryParse(dhArriverList[i], out DateTime dateTimeArriver))
                        {
                            hArriver = dateTimeArriver.TimeOfDay;
                        }
                        new CoureurTemps(idEtapeCoureurList[i], hDepart, hArriver, dhDepart, dhArriver).create(coco);
                    }
                    
                    coco.connection.Close();
                }
                catch (Exception ex)
                {
                    TempData["ErrorInsert"] = "Tsy nety inserer";
                    return RedirectToAction("affecterTemps", "Admin", new { idEtape = idEtape });
                }

            }
            return RedirectToAction("Index", "Admin");

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult import()
    {
        HttpContext.Session.Remove("sessionId");

        if(HttpContext.Session.GetString("adminId") != null)
        {
            return View("importData");

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult ImportEtapeResult(IFormFile csvFileEtape, IFormFile csvFileResult)
    {
        if (csvFileEtape != null && csvFileEtape.Length > 0 && csvFileResult != null && csvFileResult.Length > 0)
        {
            try
            {
                ImportEtape impoEtape = new ImportEtape();
                ImportResult impoResu = new ImportResult();

                Connexion coco = new Connexion();
                coco.connection.Open();
                
                string messageEtape = impoEtape.import(csvFileEtape, coco);
                string messageResult = impoResu.import(csvFileResult, coco);

                coco.connection.Close();
                
                if (messageEtape.Contains("Error") || messageEtape.Contains("Exception") || messageEtape.Contains("failed"))
                {
                    ViewBag.ErrorEtape = messageEtape;
                }
                else
                {
                    ViewBag.MessageEtape = messageEtape;
                }


                if (messageResult.Contains("Error") || messageResult.Contains("Exception") || messageResult.Contains("failed"))
                {
                    ViewBag.ErrorResult = messageResult;
                }
                else
                {
                    ViewBag.MessageResult = messageResult;
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error processing CSV file: " + ex.Message;
            }
        }
        else
        {
            ViewBag.Error = "No file selected.";
        }

        return View("importData", ViewBag);
    }

    public IActionResult importPoint()
    {
        HttpContext.Session.Remove("sessionId");

        if(HttpContext.Session.GetString("adminId") != null)
        {
            return View();

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult ImportPoint(IFormFile csvFile)
    {
        if (csvFile != null && csvFile.Length > 0)
        {
            try
            {
                ImportPoint impo = new ImportPoint();

                Connexion coco = new Connexion();
                coco.connection.Open();
                
                string messageEtape = impo.import(csvFile, coco);

                coco.connection.Close();
                
                if (messageEtape.Contains("Error") || messageEtape.Contains("Exception") || messageEtape.Contains("failed"))
                {
                    ViewBag.Error = messageEtape;
                }
                else
                {
                    ViewBag.Message = messageEtape;
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error processing CSV file: " + ex.Message;
            }
        }
        else
        {
            ViewBag.Error = "No file selected.";
        }

        return View("importData", ViewBag);
    }
}
