using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProyectoSolveCore.Models;

namespace ProyectoSolveCore.Controllers
{
    [Authorize]
    public class DepartamentoController : Controller
    {
        ModelData _context;
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
            var data = await _context.Departamentos.ToListAsync();
            data = cambiarOrden(data);

            return Content(JsonConvert.SerializeObject(new { data }, jsonSettings), "application/json");
        }

        private List<Departamento> cambiarOrden(List<Departamento> data)
        {
            var UltimoElemnto = data.Last();
            data.RemoveAt(data.Count - 1);
            data.Insert(0, UltimoElemnto);
            return data;
        }
    }
}
