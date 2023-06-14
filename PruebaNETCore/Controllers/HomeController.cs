using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoSolveCore.Filters;

namespace PruebaNETCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con la agenda.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver la agenda.
    /// </remarks>
    [Authorize]
    public class HomeController : Controller
    {
        /// <summary>
        /// Muestra la vista agenda.
        /// </summary>
        /// <returns>El resultado de la vista.</returns>
        /// <remarks>
        /// Antes de realizar la accion, se dispara un metodo llamado VerificarSolicitudes 
        /// El cual verifica todas las solicitudes y sus estados.
        /// </remarks>
        [TypeFilter(typeof(VerificarSolicitudes))]
        public IActionResult Agenda()
        {
            return View();
        }
    }
}