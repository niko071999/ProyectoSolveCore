using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoSolveCore.Models;
using PruebaNETCore.Models;
using System.Diagnostics;

namespace PruebaNETCore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ModelData _db;

        public HomeController(ModelData db)
        {
            _db = db;
        }
        public IActionResult Agenda()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}