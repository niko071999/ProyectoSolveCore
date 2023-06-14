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
        private readonly ModelData _context;
        public DepartamentoController(ModelData context)
        {
            _context = context;
        }
        public IActionResult VisualizarDepartamentos()
        {
            return View();
        }

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
