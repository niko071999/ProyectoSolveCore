using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con la generacion del documento Permiso de Circulación.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver y crear el documento de Permiso de Circulación.
    /// </remarks>
    public class GenerateDocumentController : Controller
    {
        private readonly ModelData _context;

        public GenerateDocumentController(ModelData contex)
        {
            _context = contex;
        }
        public async Task<PartialViewResult> SeleccionarFirmador(int id = 0)
        {
            if (id == 0)
            {
                string mensaje = "Hubo un error al enviar los datos";
                return PartialView("_PartialModalError", mensaje);
            }
            var conductores = await _context.Conductores.Include(c => c.IdUsuarioNavigation)
                .Where(c => !c.Eliminado).ToListAsync();

            var solicitud = await _context.Solicitudes.Where(s => s.Id == id)
                .FirstOrDefaultAsync();
            var conductor = conductores.FirstOrDefault(c => c.Id == solicitud.IdConductor);

            ViewBag.IdConductor = new SelectList(GetSelectListItemConductores(conductores), "Value", "Text", solicitud.IdConductor.HasValue 
                    ? solicitud.IdConductor : null);
            int solicitudId = solicitud.Id;
            
            return PartialView("_SeleccionarFirmador", solicitudId);

        }

        private static List<SelectListItem> GetSelectListItemConductores(List<Conductore> conductores)
        {

            return conductores.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.IdUsuarioNavigation.Nombre} {c.IdUsuarioNavigation.Apellido}"
            }).ToList();
        }

        public async Task<IActionResult> PermisoCirculacion(string nombre, int idconductor, int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("MisSolicitudes");
            }
            var conductor = await _context.Conductores.Include(c => c.IdUsuarioNavigation)
                    .Where(c => c.Id == idconductor).FirstOrDefaultAsync();
            string NombreConductor = $"{conductor.IdUsuarioNavigation.Nombre} {conductor.IdUsuarioNavigation.Apellido}";

            var solicitud = await _context.Solicitudes.Where(s => s.Id == id)
                .Select(s => new vmPermisoCirculacion()
                {
                    vehiculo = s.IdVehiculoNavigation.Patente,
                    nombreConductor = NombreConductor,
                    pasajeros = ModificarTextoPasajeros(s.Pasajeros),
                    motivo = s.Motivo,
                    destino = s.Destino,
                    FechaLlegada = s.FechaLlegada,
                    FechaSalida = s.FechaSalida,
                    NombreFirma = nombre
                }).FirstOrDefaultAsync();
            if (solicitud == null)
            {
                return View(new vmPermisoCirculacion());
            }

            return View(solicitud);
        }

        private static string ModificarTextoPasajeros(string pasajeros)
        {
            string[] nombres = pasajeros.Split(',');

            for (int i = 0; i < nombres.Length; i++)
            {
                nombres[i] = nombres[i].Trim();
                if (i % 2 == 0)
                {
                    nombres[i] += ",";
                }
                else
                {
                    nombres[i] += "<br />";
                }
            }

            string resultado = string.Join(" ", nombres);
            return resultado;
        }
    }
}
