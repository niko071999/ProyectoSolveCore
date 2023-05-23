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
                List<SelectListGroup> groups = IniciarInstanciasGroups();
                var vlist = await _context.Vehiculos.Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado).Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                }).ToListAsync();
                var clist = await _context.Conductores.Include(c => c.IdUsuarioNavigation).Where(c => !c.Eliminado).Select(c => new SelectListWithGroups()
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

        private List<SelectListGroup> IniciarInstanciasGroups()
        {
            List<SelectListGroup> groups = _context.Categorias.Select(c => new SelectListGroup(){
                Name = c.Categoria1
            }).ToList();
            return groups;
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
                if (n == 0)
                {
                    return View(fm);
                }
                return RedirectToAction("VisualizarMantenciones");
            }
            catch (Exception ex)
            {
                return RedirectToAction("VisualizarMantenciones");
            }
        }
    }
}
