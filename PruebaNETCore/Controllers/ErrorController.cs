using Microsoft.AspNetCore.Mvc;

namespace ProyectoSolveCore.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error404()
        {
            return View();
        }
        public IActionResult Error401()
        {
            return View();
        }
    }
}
