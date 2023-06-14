using Microsoft.AspNetCore.Mvc;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja los errores en el sistema.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver errores del sistema.
    /// </remarks>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Acción del controlador que muestra el error 404.
        /// </summary>
        /// <returns>El resultado de la vista.</returns>
        public IActionResult Error404()
        {
            return View();
        }
        /// <summary>
        /// Acción del controlador que muestra el error 401.
        /// </summary>
        /// <returns>El resultado de la vista.</returns>
        public IActionResult Error401()
        {
            return View();
        }
    }
}
