using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProyectoSolveCore.Models;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con el Departamento.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver y conseguir registros de la bitácora.
    /// </remarks>
    [Authorize]
    public class DepartamentoController : Controller
    {
        /// <summary>
        /// contexto de la base de datos
        /// </summary>
        private readonly ModelData _context;
        //Constructor
        public DepartamentoController(ModelData context)
        {
            _context = context;
        }
        public IActionResult VisualizarDepartamentos() => View();
        /// <summary>
        /// Método que obtiene los departamentos para cargarlo en la etiqueta Select HTML 
        /// </summary>
        /// <returns>Un json con todos los departamentos para cargarlos en un Select HTML</returns>
        public async Task<IActionResult> GetDepartamentos()
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var data = await _context.Departamentos.OrderBy(d => d.Departamento1).ToListAsync();

            return Content(JsonConvert.SerializeObject(new { data }, jsonSettings), "application/json");
        }
    }
}
