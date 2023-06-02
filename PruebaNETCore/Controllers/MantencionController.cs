using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;
using System.Globalization;

namespace ProyectoSolveCore.Controllers
{
    public class MantencionController : Controller
    {
        private readonly ModelData _context;
        public MantencionController(ModelData context)
        {
            _context = context;
        }
        [Autorizar(16)]
        public async Task<IActionResult> VisualizarMantenciones()
        {
            var mantenciones = await _context.Fichamantencions.Include(m => m.IdConductorNavigation.IdUsuarioNavigation).Include(m => m.IdVehiculoNavigation)
                .ToListAsync();
            return View(mantenciones);
        }
        public async Task<IActionResult> AgregarEntradaFM(int id = -1)
        {
            if (id == -1)//Esto quiere decir que viene nulo el id
            {
                return RedirectToAction("VisualizarMantenciones");
            }
            try
            {
                var vehiculos = await _context.Vehiculos.Include(v => v.IdCategoriaNavigation).Where(v => !v.Eliminado).ToListAsync();

                var vlist = new List<SelectListWithGroups>();
                //Verifica si el usuario es mantenedor de vehiculos normales para mostrar solo las categorias correspondiente para el rol
                if (User.IsInRole("Mantenedor de vehículos no pesados") && User.IsInRole("Mantenedor de vehículos pesados") || User.IsInRole("Administrador"))
                {
                    vlist = vehiculos.Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    }).ToList();
                }else if (User.IsInRole("Mantenedor de vehículos no pesados"))  
                {
                    vlist = vehiculos.Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    })
                    .Where(vl => vl.Group.Equals("SUV") || vl.Group.Equals("Camioneta")
                    || vl.Group.Equals("Auto") || vl.Group.Equals("Furgon"))
                    .ToList();
                }
                else
                {
                    vlist = vehiculos.Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    })
                    .Where(v => v.Group.Equals("Camion") || v.Group.Equals("Retroexcavadora")
                            || v.Group.Equals("Bus") || v.Group.Equals("Tractor"))
                    .ToList();
                }

                var clist = await _context.Conductores.Include(c => c.IdUsuarioNavigation)
                    .Where(c => !c.Eliminado)
                    .Select(c => new SelectListWithGroups()
                    {
                        Value = c.Id.ToString(),
                        Text = $"{c.IdUsuarioNavigation.Nombre} {c.IdUsuarioNavigation.Apellido}"
                    }).ToListAsync();
                if (id == 0)
                {
                    var v = vehiculos.FirstOrDefault(v => int.Parse(vlist.FirstOrDefault().Value) == v.Id);
                    ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", null, "Group");
                    ViewBag.IdConductor = new SelectList(clist, "Value", "Text", v.IdConductor??int.Parse(clist.FirstOrDefault().Value));
                }
                else
                {
                    var v = vehiculos.FirstOrDefault(v => v.Id == id);
                    if (v.IdConductor.HasValue)
                    {
                        ViewBag.IdConductor = new SelectList(clist, "Value", "Text", v.IdConductor);
                    }
                    else
                    {
                        ViewBag.IdConductor = new SelectList(clist, "Value", "Text");

                    }
                    ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", id, "Group");
                }
                var veh = vehiculos.FirstOrDefault();
                if (veh.IdConductor.HasValue)
                {
                    ViewBag.IdConductor = new SelectList(clist, "Value", "Text", veh.IdConductor);
                }
                else
                {
                    ViewBag.IdConductor = new SelectList(clist, "Value", "Text");
                }
                return View(new vmFichaMantencion());
            }
            catch (Exception ex)
            {
                return RedirectToAction("VisualizarMantenciones");
            }
        }
        [Autorizar(14)]
        [HttpPost]
        public async Task<IActionResult> AgregarEntradaFM(vmFichaMantencion fm)
        {
            if (fm == null)
            {
                return View(new vmFichaMantencion());
            }
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                Fichamantencion ficha = new()
                {
                    FechaMantencion = GenerarFecha(DateTime.Now),
                    Kilometraje = fm.Kilometraje,
                    Descripcion = fm.Descripcion,
                    IdConductor = fm.IdConductor,
                    IdVehiculo = fm.IdVehiculo
                };
                await _context.Fichamantencions.AddAsync(ficha);
                int n = await _context.SaveChangesAsync();
                var v = await _context.Vehiculos.FindAsync(fm.IdVehiculo);
                if (v == null) return View(fm);
                if (!v.Estado)
                {
                    v.Estado = true;
                    n = await _context.SaveChangesAsync();
                }
                if (n == 0) return View(fm);
                await transaction.CommitAsync();
                return RedirectToAction("VisualizarMantenciones");
            }
            catch (Exception ex)
            {
                return RedirectToAction("VisualizarMantenciones");
            }
        }

        private DateTime GenerarFecha(DateTime now)
        {
            var hoystr = now.ToString("dd-MM-yyyy HH:mm:ss");
            return DateTime.ParseExact(hoystr, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
