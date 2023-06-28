using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoSolveCore.Filters;

namespace PruebaNETCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con el inicio del sistema y la agenda.
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        /// <summary>
        /// Muestra la vista agenda.
        /// </summary>
        /// <returns>El resultado de la vista.</returns>
        /// <remarks>
        /// Antes de realizar la acción, se dispara un método llamado VerificarSolicitudes 
        /// El cual verifica todas las solicitudes y sus estados.
        /// </remarks>
        [TypeFilter(typeof(VerificarSolicitudes))]
        public IActionResult Agenda()
        {
            return View();
        }
    }
}