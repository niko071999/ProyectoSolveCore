using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;

namespace ProyectoSolveCore.Controllers
{
    public class MantencionController : Controller
    {
        private readonly ModelData _context;
        public MantencionController(ModelData context)
        {
            _context = context;
        }
        public async Task<IActionResult> VisualizarMantenciones()
        {
            var mantenciones = await _context.FichaMantencions.Include(m => m.IdConductorNavigation.IdUsuarioNavigation).Include(m => m.IdVehiculoNavigation)
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
                var vlist = await _context.Vehiculos.Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado)
                    .Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                }).ToListAsync();
                //Verifica si el usuario es mantenedor de vehiculos normales para mostrar solo las categorias correspondiente para el rol
                if (User.IsInRole("Mantenedor de vehículos no pesados"))  
                {
                    vlist = vlist.Where(vl => vl.Group.Equals("SUV") || vl.Group.Equals("Camioneta") 
                        || vl.Group.Equals("Auto") || vl.Group.Equals("Furgon"))
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
                    ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", null, "Group");
                }
                else
                {
                    ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", id, "Group");
                }
                ViewBag.IdConductor = new SelectList(clist, "Value", "Text");
                return View(new vmFichaMantencion());
            }
            catch (Exception ex)
            {
                return RedirectToAction("VisualizarMantenciones");
            }
        }

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
                FichaMantencion ficha = new()
                {
                    FechaMantencion = DateTime.Now,
                    Kilometraje = fm.Kilometraje,
                    Descripcion = fm.Descripcion,
                    IdConductor = fm.IdConductor,
                    IdVehiculo = fm.IdVehiculo
                };
                await _context.FichaMantencions.AddAsync(ficha);
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
    }
}
