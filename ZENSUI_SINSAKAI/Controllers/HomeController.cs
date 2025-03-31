using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using ZENSUI_SINSAKAI.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZENSUI_SINSAKAI.Controllers;

public class HomeController : Controller
{
    private int _sinsakaiId = 1;
    private int _userId = 1;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        SinsakaiModel sinsakaiModel = new SinsakaiModel();
        Sinsakai sinsakai = sinsakaiModel.GetSinsakai(_sinsakaiId, _userId);

        return View(sinsakai);
    }

    public IActionResult Syousai(int id, int id2, int id3)
    {
        SinsakaiModel sinsakaiModel = new SinsakaiModel();
        Syuppin syuppin = sinsakaiModel.GetSyuppin(id, id2, id3);

        return View(syuppin);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public ActionResult ReadImage(int sinsakaiId, int syuppinNo, int saitensyaId)
    {
        SinsakaiModel sinsakaiModel = new SinsakaiModel();
        string? image = sinsakaiModel.ReadImage(sinsakaiId, syuppinNo, saitensyaId);

        return new JsonResult(image);
    }

    [HttpPost]
    public ActionResult UploadImage(int sinsakaiId, int syuppinNo, int saitensyaId, string data)
    {
        SinsakaiModel sinsakaiModel = new SinsakaiModel();
        bool result = sinsakaiModel.UploadImage(sinsakaiId, syuppinNo, saitensyaId, data);

        return View();
    }

    [HttpGet]
    public ActionResult GetScore(int sinsakaiId, int syuppinNo, int saitensyaId)
    {
        SinsakaiModel sinsakaiModel = new SinsakaiModel();
        Score score = sinsakaiModel.GetScore(sinsakaiId, syuppinNo, saitensyaId);

        return new JsonResult(score);
    }

    [HttpPost]
    public ActionResult RegistScore(int sinsakaiId, int syuppinNo, int saitensyaId, int? scoreKannouTokusei, int? scoreTyakusou, int? scoreGenryou, int? scoreGizyutu, int? scoreHousou)
    {
        SinsakaiModel sinsakaiModel = new SinsakaiModel();
        bool result = sinsakaiModel.RegistScore(sinsakaiId, syuppinNo, saitensyaId, scoreKannouTokusei, scoreTyakusou, scoreGenryou, scoreGizyutu, scoreHousou);

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
