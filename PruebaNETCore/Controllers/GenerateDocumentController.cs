using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;

namespace ProyectoSolveCore.Controllers
{
    public class GenerateDocumentController : Controller
    {
        private readonly ModelData _context;
        private readonly IConverter _converter;

        public GenerateDocumentController(ModelData contex, IConverter converter)
        {
            _context = contex;
            _converter = converter;
        }
        public async Task<PartialViewResult> SeleccionarFirmador(int id = 0)
        {
            if (id == 0)
            {
                string mensaje = "Hubo un error al enviar los datos";
                return PartialView("_PartialModalError", mensaje);
            }
            int solicitudId = await _context.Solicitudes.Select(s => s.Id).FirstOrDefaultAsync(s => s == id);
            return PartialView("_SeleccionarFirmador", solicitudId);

        }
        [HttpPost]
        public PartialViewResult MostrarPermisoCirculacionModal([FromBody] vmPermisoCirculacionModal pcm)
        {
            if (pcm == null)
            {
                string mensaje = "Hubo un error al enviar los datos";
                return PartialView("_PartialModalError", mensaje);
            }
            return PartialView("_VerPermisoCirculacion", pcm);
        }
        public IActionResult MostrarPermisoCirculacionPDF(string nombre, int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }
            string UrlPagina = Url.Action("PermisoCirculacion", "GenerateDocument", new { id = id, nombre = nombre }, Request.Scheme);

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        Page = UrlPagina
                    }
                }
            };

            var archivo = _converter.Convert(pdf);
            return File(archivo, "application/pdf");
        }
        public async Task<IActionResult> PermisoCirculacion(string nombre, int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("MisSolicitudes");
            }
            var solicitud = await _context.Solicitudes.Where(s => s.Id == id)
                .Select(s => new vmPermisoCirculacion()
                {
                    vehiculo = s.IdVehiculoNavigation.Patente,
                    nombreConductor = $"{s.IdConductorNavigation.IdUsuarioNavigation.Nombre} {s.IdConductorNavigation.IdUsuarioNavigation.Apellido}",
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
