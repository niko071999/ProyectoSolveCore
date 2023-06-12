using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using System.Globalization;
using System.Linq;

namespace ProyectoSolveCore.Controllers
{
    [Authorize]
    public class KilometrajeController : Controller
    {
        private readonly ModelData _context;

        public KilometrajeController(ModelData context)
        {
            _context = context;
        }

        // GET: KilometrajeController
        [Autorizar(20)]
        public async Task<IActionResult> VisualizarHistorialKm()
        {
            var vlist = await ObtenerVehiculos();
            ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", null, "Group");
            return View(new List<Kilometraje>());
        }
        [Autorizar(20)]
        [HttpPost]
        public async Task<IActionResult> VisualizarHistorialKm(Kilometraje km)
        {
            var kilometros = await _context.Kilometrajes.Where(k => k.IdVehiculo == km.IdVehiculo).Include(k => k.IdVehiculoNavigation)
                .ToListAsync();
            var vlist = await ObtenerVehiculos();
            ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", km.IdVehiculo, "Group");
            return View(kilometros);
        }
        [Autorizar(21)]
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
        [Autorizar(21)]
        [HttpPost]
        public async Task<IActionResult> AgregarKilometrajeVehiculo(Kilometraje k)
        {
            var vehiculos = await ObtenerVehiculos();
            if (k == null)
            {
                ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", null, "Group");
                return View(new Kilometraje());
            }
            if (k.KilometrajeInicial > k.KilometrajeFinal || k.KilometrajeInicial < 0 || k.KilometrajeFinal < 0)
            {
                ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", k.IdVehiculo, "Group");
                return View(k);
            }
            k.FechaCreacion = DateTime.Now;
            await _context.Kilometrajes.AddAsync(k);
            int n = await _context.SaveChangesAsync();
            if (n == 0)
            {
                ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", k.IdVehiculo, "Group");
                return View(k);
            }
            bool check = await VerficairKmVehiculos(k.IdVehiculo);

            return RedirectToAction("VisualizarHistorialKm");
        }

        private Task<bool> VerficairKmVehiculos(int idVehiculo)
        {
            throw new NotImplementedException();
        }

        private static DateTime GenerateFecha(DateTime now)
        {
            var hoystr = now.ToString("dd-MM-yyyy HH:mm:ss");
            return DateTime.ParseExact(hoystr, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private async Task<List<SelectListWithGroups>> ObtenerVehiculos()
        {
            var categorias = _context.Categorias.OrderBy(c => c.Categoria1).ToList();
            var sli = new List<SelectListItem>();

            if (User.IsInRole("Mantenedor de vehículos pesados") && User.IsInRole("Mantenedor de vehículos no pesados") || User.IsInRole("Administrador"))
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
            else if (User.IsInRole("Mantenedor de vehículos no pesados") || User.IsInRole("Administrador"))
            {
                return await _context.Vehiculos.Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado)
                    .Where(v => v.IdCategoriaNavigation.Categoria1.Equals("SUV") || v.IdCategoriaNavigation.Categoria1.Equals("Camioneta")
                            || v.IdCategoriaNavigation.Categoria1.Equals("Auto") || v.IdCategoriaNavigation.Categoria1.Equals("Furgon"))
                    .Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    }).ToListAsync();
            }
            else
            {
                return await _context.Vehiculos.Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado)
                    .Where(v => v.IdCategoriaNavigation.Categoria1.Equals("Camion") || v.IdCategoriaNavigation.Categoria1.Equals("Retroexcavadora")
                            || v.IdCategoriaNavigation.Categoria1.Equals("Bus") || v.IdCategoriaNavigation.Categoria1.Equals("Tractor"))
                    .Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    }).ToListAsync();
            }
        }
    }
}
