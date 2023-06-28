using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con la generación del documento Permiso de Circulación.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver y crear el documento de Permiso de Circulación.
    /// </remarks>
    public class GenerateDocumentController : Controller
    {
        /// <summary>
        /// contexto de la base de datos
        /// </summary>
        private readonly ModelData _context;

        public GenerateDocumentController(ModelData contex)
        {
            _context = contex;
        }
        /// <summary>
        /// Acción que devuelve una vista parcial para seleccionar un firmador y conductor en una ventana emergente.
        /// </summary>
        /// <param name="id">ID de la solicitud.</param>
        /// <returns>Vista parcial para cargar en un modal.</returns>
        public async Task<PartialViewResult> SeleccionarFirmador(int id = 0)
        {
            if (id == 0)
            {
                string mensaje = "Hubo un error al enviar los datos";
                return PartialView("_PartialModalError", mensaje);
            }
            var conductores = await _context.Conductores.Include(c => c.IdUsuarioNavigation)
                .Where(c => !c.Eliminado || c.Estado).ToListAsync();

            var solicitud = await _context.Solicitudes.Where(s => s.Id == id)
                .FirstOrDefaultAsync();
            var conductor = conductores.FirstOrDefault(c => c.Id == solicitud.IdConductor);

            ViewBag.IdConductor = new SelectList(GetSelectListItemConductores(conductores), "Value", "Text", solicitud.IdConductor.HasValue 
                    ? solicitud.IdConductor : null);
            int solicitudId = solicitud.Id;
            
            return PartialView("_SeleccionarFirmador", solicitudId);
        }
        /// <summary>
        /// Selecciona los conductores disponibles para el seleccionar.
        /// </summary>
        /// <param name="conductores">Lista de conductores.</param>
        /// <returns>Lista de conductores para cargarlos en una etiqueta Select HTML.</returns>
        private static List<SelectListItem> GetSelectListItemConductores(List<Conductor> conductores)
        {
            return conductores.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.IdUsuarioNavigation.Nombre} {c.IdUsuarioNavigation.Apellido}"
            }).ToList();
        }
        /// <summary>
        /// Acción que devuelve el documento generado con los datos de la solicitud.
        /// </summary>
        /// <param name="nombre">Nombre del firmador seleccionado.</param>
        /// <param name="idconductor">ID del conductor seleccionado.</param>
        /// <param name="id">ID de la solicitud a generar.</param>
        /// <returns>Lista de conductores para cargarlos en una etiqueta Select HTML.</returns>
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
        /// <summary>
        /// Modifica la cadena de texto de los nombres de los pasajeros.
        /// </summary>
        /// <remarks>Al haber mas de 2 nombre de pasajeros, 
        /// lo divide en dos partes para que puedan caer en la generación del documento</remarks>
        /// <param name="pasajeros">Nombres de los pasajeros.</param>
        /// <returns>Nombres de los pasajeros modificada.</returns>
        private static string ModificarTextoPasajeros(string pasajeros)
        {
            //Divido la cadena de texto por cada coma que halla.
            string[] nombres = pasajeros.Split(',');
            for (int i = 0; i < nombres.Length; i++)
            {
                nombres[i] = nombres[i].Trim();
                //Se verifica si es numero par
                if (i % 2 == 0)
                {//Si es, se concatena una coma
                    nombres[i] += ",";
                }
                else
                {//Si no, se concatena la etiqueta <br />
                    nombres[i] += "<br />";
                }
            }
            //Se junta dejando un espacio por cada nombre y coma.
            return string.Join(" ", nombres);
        }
    }
}
