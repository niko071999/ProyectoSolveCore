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
        [TypeFilter(typeof(VerificarSolicitudes))]
        public IActionResult Agenda()
        {
            return View();
        }
    }
}