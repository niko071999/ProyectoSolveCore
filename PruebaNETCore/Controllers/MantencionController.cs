using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;

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
            if (id == -1)
            {
                return RedirectToAction("VisualizarMantenciones");
            }

            return View();
        }
    }
}
