using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoSolveCore.Filters;
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
        [TypeFilter(typeof(VerificarSolicitudes))]
        public IActionResult Agenda()
        {
            return View();
        }
    }
}