﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Race.Models;

namespace Race.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult checkLog()
    {
        HttpContext.Session.Remove("sessionId");
        HttpContext.Session.Remove("adminId");

        var username = Request.Form["username"].ToString();
        var passW = Request.Form["password"].ToString();

        Uuser uu = new Uuser();

        Connexion coco = new Connexion();
        coco.connection.Open();
        
        string idUser = uu.checkLogin(coco, username, passW);
        if (idUser != null)
        {
            bool isAdmin = uu.isAdmin(coco, idUser);

            if (isAdmin)
            {
                coco.connection.Close();
                HttpContext.Session.SetString("adminId", idUser);
                return RedirectToAction("Index", "Admin");

            }else{

                coco.connection.Close();
                HttpContext.Session.SetString("sessionId", idUser);
                return RedirectToAction("listeEtape", "Home");
            }

        }else{
            TempData["error"] = "Misy diso ny user na mdp";
            coco.connection.Close();
            return RedirectToAction("Index", "Home", new { error = true });
        }

        coco.connection.Close();

        return View();
    }

    public IActionResult ClearSession()
    {
        HttpContext.Session.Remove("sessionId");
        HttpContext.Session.Remove("adminId");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult listeEtape()
    {
        HttpContext.Session.Remove("adminId");

        if(HttpContext.Session.GetString("sessionId") != null)
        {
            Etape e = new Etape();
            Data data = new Data();
            
            Connexion coco = new Connexion();
            coco.connection.Open();

            data.etapeList = e.findAll(coco);
            data.chronos = Result.chronos(coco, HttpContext.Session.GetString("sessionId"));

            coco.connection.Close();

            return View("listeEtape", data);

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult affecterCoureur(string idEtape)
    {
        HttpContext.Session.Remove("adminId");

        if(HttpContext.Session.GetString("sessionId") != null)
        {
            // var idEtape = Request.Form["idEtape"].ToString();

            Data data = new Data();
            
            Connexion coco = new Connexion();
            coco.connection.Open();

            data.etape = new Etape().findById(coco, idEtape);
            data.coureurList = new Coureur().findByIdEquipe(coco, HttpContext.Session.GetString("sessionId"));

            coco.connection.Close();

            if (TempData["ErrorAffecte"] != null)
            {
                ViewBag.Error = TempData["ErrorAffecte"];
            }
            return View("AffecterCoureur", data);

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult affecte()
    {
        HttpContext.Session.Remove("adminId");

        if(HttpContext.Session.GetString("sessionId") != null)
        {
            var idEtape = Request.Form["idEtape"].ToString();

            // var coureurs = Request.Form["coureurs"].ToString().Split(',');
            StringValues coureursValues;
                var coureursExist = Request.Form.TryGetValue("coureurs", out coureursValues);
                var coureurs = coureursExist ? coureursValues.ToString().Split(',') : Array.Empty<string>();

            var limit = int.Parse(Request.Form["limit"].ToString());

            if (coureurs.Length > limit)
            {
                TempData["ErrorAffecte"] = "Tsy azo atao mihotra na tsy ampy amn nombre coureur";
                return RedirectToAction("affecterCoureur", "Home", new { idEtape = idEtape });

            }else{
                Connexion coco = new Connexion();
                coco.connection.Open();

                for (int i = 0; i < coureurs.Length; i++)
                {
                    Console.WriteLine(coureurs[i]);
                    new Coureur(idEtape, HttpContext.Session.GetString("sessionId"), coureurs[i]).createEtapeCoureur(coco);
                }

                coco.connection.Close();
                return RedirectToAction("listeEtape", "Home");
            }

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult classement(string category)
    {
        if(HttpContext.Session.GetString("sessionId") != null || HttpContext.Session.GetString("adminId") != null)
        {
            Data data = new Data();
            Connexion coco = new Connexion();
            coco.connection.Open();

            if (category != null)
            {
                data.v_result_category = new Result().v_result_category(coco, category);
            }else
            {
                data.v_result_category = new Result().v_result_category(coco, "cat1");
            }
            data.resultList = Result.findAll(coco);
            data.CGPointEtape = Result.CGPointEtape(coco);
            data.CG = Result.CG(coco);
            data.CGCoureur = Result.CGCoureur(coco);
            data.categoryList = new Coureur().listCAtegory(coco);

            coco.connection.Close();
            return View("result", data);

        }else{

            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult aleas5(string equipe)
    {
        if(HttpContext.Session.GetString("sessionId") != null || HttpContext.Session.GetString("adminId") != null)
        {
            Data data = new Data();
            Connexion coco = new Connexion();

            coco.connection.Open();
            data.resultList = Result.aleas5(coco, equipe);

            coco.connection.Close();
            return View("aleas5", data);

        }else{

            return RedirectToAction("Index", "Home");
        }
    }
}
