using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;

namespace ProyectoSolveCore.Controllers
{
    public class KilometrajeController : Controller
    {
        private readonly ModelData _context;

        public KilometrajeController(ModelData context)
        {
            _context = context;
        }

        // GET: KilometrajeController
        public async Task<IActionResult> VisualizarHistorialKm()
        {
            var vlist = await ObtenerVehiculos();
            ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", null, "Group");
            return View(new List<Kilometraje>());
        }
        [HttpPost]
        public async Task<IActionResult> VisualizarHistorialKm(Kilometraje km)
        {
            var kilometros = await _context.Kilometrajes.Where(k => k.IdVehiculo == km.IdVehiculo).Include(k => k.IdVehiculoNavigation)
                .ToListAsync();
            var vlist = await ObtenerVehiculos();
            ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", km.IdVehiculo, "Group");
            return View(kilometros);
        }
        public async Task<IActionResult> AgregarKilometrajeVehiculo(int id = -1)
        {
            if (id == -1) //Si es -1 es nulo
            {
                return RedirectToAction("VisualizarHistorialKm");
            }
            var vehiculos = await ObtenerVehiculos();
            if (id == 0)
            {
                ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", null, "Group");
                return View(new Kilometraje());
            }
            ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", id, "Group");
            return View(new Kilometraje());
        }
        [HttpPost]
        public async Task<IActionResult> AgregarKilometrajeVehiculo(Kilometraje k)
        {
            var vehiculos = await ObtenerVehiculos();
            if (k == null)
            {
                ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", null, "Group");
                return View(new Kilometraje());
            }
            k.FechaCreacion = DateTime.Now;
            await _context.Kilometrajes.AddAsync(k);
            int n = await _context.SaveChangesAsync();
            if (n == 0)
            {
                ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", k.IdVehiculo, "Group");
                return View(k);
            }
            return RedirectToAction("VisualizarHistorialKm");
        }
        private async Task<List<SelectListWithGroups>> ObtenerVehiculos()
        {
            return await _context.Vehiculos.Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado)
                    .Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    }).ToListAsync();
        }
    }
}
