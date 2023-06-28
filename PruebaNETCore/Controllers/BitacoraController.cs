using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;
using System.Globalization;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con la bitácora.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver, crear, modificar y eliminar registros de la bitácora.
    /// </remarks>
    [Authorize]
    public class BitacoraController : Controller
    {
        /// <summary>
        /// contexto de la base de datos
        /// </summary>
        private readonly ModelData _context;
        /// <summary>
        /// Memoria cache
        /// </summary>
        private readonly IMemoryCache _cache;
        //Constructor
        public BitacoraController(ModelData context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        /// <summary>
        /// Muestra todos los registros de la bitácora realizadas.
        /// </summary>
        /// <returns>Una vista que muestra todos los registros realizados.</returns>
        [Autorizar(permisoId:19)]
        public async Task<IActionResult> VisualizarBitacora()
        {
            var bitacoras = await _context.Bitacoras.Include(b => b.IdSolicitudNavigation).Include(b => b.IdVehiculoNavigation)
                .Include(b => b.IdConductorNavigation.IdUsuarioNavigation).Include(b => b.IdKilometrajeNavigation)
                .ToListAsync();
            return View(bitacoras);
        }
        /// <summary>
        /// Muestra una vista con un formulario para ingresar un registro a la bitácora.
        /// </summary>
        /// <param name="id">ID de la solicitud a ingresar</param>
        /// <returns>Una vista que muestra un formulario para ingresar un registro a la bitácora.</returns>
        [Autorizar(permisoId:1)]
        public async Task<IActionResult> AgregarEntradasBitacora(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return RedirectToAction("MisSolicitudes", "Solicitud");
                }
                var solicitudes = await _context.Solicitudes.Where(s => s.Estado == 1)
                    .Include(s => s.IdConductorNavigation.IdUsuarioNavigation).Include(s => s.IdVehiculoNavigation).ToListAsync();
                vmBitacora bitacora = solicitudes.Select(s => new vmBitacora()
                {
                    FechaSalida = s.FechaSalida,
                    FechaLlegada = s.FechaLlegada,
                    Destino = s.Destino,
                    IdConductor = (int)s.IdConductor,
                    IdSolicitud = s.Id,
                    IdVehiculo = s.IdVehiculo,
                    Motivo = s.Motivo,
                    NombreCompletoConductor = s.IdConductorNavigation.IdUsuarioNavigation.Nombre + " " + s.IdConductorNavigation.IdUsuarioNavigation.Apellido,
                    Vehiculo = s.IdVehiculoNavigation.Patente + " " + s.IdVehiculoNavigation.Marca + " " + s.IdVehiculoNavigation.Modelo
                }).FirstOrDefault(s => s.IdSolicitud == id);
                if (bitacora == null)
                {
                    return RedirectToAction("MisSolicitudes", "Solicitud");
                }
                return View(bitacora);
            }
            catch (Exception)
            {
                return RedirectToAction("MisSolicitudes", "Solicitud");
            }
        }
        /// <summary>
        /// Acción que permite agregar varias entradas a la bitácora.
        /// </summary>
        /// <param name="bitacora">Los datos de la bitácora a agregar.</param>
        /// <returns>Vista que representa el resultado de la operación.</returns>
        [Autorizar(permisoId: 1)]
        [HttpPost]
        public async Task<IActionResult> AgregarEntradasBitacora(vmBitacora bitacora) //Esta acción permite la inserción de varias entradas a la bitácora.
        {
            if (bitacora == null)
            {
                return RedirectToAction("MisSolicitudes","Solicitudes");
            }
            var checkSol = await _context.Bitacoras.AnyAsync(b => b.IdSolicitud == bitacora.IdSolicitud);
            if (checkSol) //Verifica si existe la solicitud ya ingresada en la bitácora
            {
                return RedirectToAction("HubSolicitudes", "Solicitud");
            }
            try
            {
                bool valid = await AgregarEntrada(bitacora);

                if (!valid)
                {
                    return View(bitacora);
                }
                return RedirectToAction("HubSolicitudes", "Solicitud");
            }
            catch (Exception ex)
            {
                return View(bitacora);
            }
        }
        /// <summary>
        /// Acción que permite agregar una entrada a la bitácora.
        /// </summary>
        /// <param name="bitacora">Los datos de la bitácora a agregar.</param>
        /// <returns>Vista que representa el resultado de la operación.</returns>
        [Autorizar(permisoId:1)]
        [HttpPost]
        public async Task<IActionResult> AgregarEntradaBitacora(vmBitacora bitacora)
        {

            if (bitacora == null)
            {
                return RedirectToAction("VisualizarBitacora", "Bitácora");
            }
            var checkSol = await _context.Bitacoras.AnyAsync(b => b.IdSolicitud == bitacora.IdSolicitud);
            if (checkSol) //Verifica si existe la solicitud ya ingresada en la bitácora
            {
                return RedirectToAction("HubSolicitudes", "Solicitud");
            }
            try
            {
                bool valid = await AgregarEntrada(bitacora);

                if (!valid)
                {
                    return View(bitacora);
                }
                EliminarMemoriaCache();
                return RedirectToAction("VisualizarBitacora", "Bitácora");
            }
            catch (Exception)
            {
                return View(bitacora);
            }
        }
        /// <summary>
        /// Elimina los elementos de la memoria cache.
        /// </summary>
        private void EliminarMemoriaCache()
        {
            if (_cache.TryGetValue("NumeroSolicitudes", out int _))
            {
                _cache.Remove("NumeroSolicitudes");
            }
        }
        /// <summary>
        /// Agrega una nueva entrada de bitácora y actualiza los registros relacionados, como los registros de kilometraje, estado del vehículo y estado de la solicitud.
        /// </summary>
        /// <param name="bitacora">Los datos de la bitácora a agregar.</param>
        /// <returns>Devuelve un valor booleano que indica si la entrada de bitácora se agregó correctamente.</returns>
        private async Task<bool> AgregarEntrada(vmBitacora bitacora)
        {
            try
            {
                //Se verifica si el km inicial es mayor a km final
                if (bitacora.KmInicialEntero > bitacora.KmFinalEntero)
                {
                    return false;
                }
                //Se inicia la transacción a la base de datos
                using var transaction = await _context.Database.BeginTransactionAsync();

                var km = new Kilometraje()
                {
                    IdVehiculo = bitacora.IdVehiculo,
                    KilometrajeInicial = bitacora.KmInicialEntero,
                    KilometrajeFinal = bitacora.KmFinalEntero,
                    FechaCreacion = DateTime.Now
                };
                await _context.Kilometrajes.AddAsync(km);
                await _context.SaveChangesAsync();
                //Se verifica el estado del vehículo
                bool statusVehiculo = VerificarMantenimientoVehiculo(bitacora.IdVehiculo);
                int n = 0;
                if (!statusVehiculo)
                {//Si el vehículo esta deshabilitado, se busca, para cambiar estado
                 //y se guarda los cambios en la base de datos
                    var vehiculo = _context.Vehiculos.Find(bitacora.IdVehiculo);
                    vehiculo.Estado = false;
                    _context.Vehiculos.Update(vehiculo);
                    n = await _context.SaveChangesAsync();
                    if (n == 0)
                    {
                        return false;
                    }
                }

                var b = new Bitacora()
                {
                    Folio = _context.Bitacoras.LongCount() + 1,
                    Fecha = DateTime.Now,
                    LitrosCombustible = bitacora.Combustible,
                    IdConductor = bitacora.IdConductor,
                    IdKilometraje = km.Id,
                    IdSolicitud = bitacora.IdSolicitud,
                    IdVehiculo = bitacora.IdVehiculo,
                    Observacion = bitacora.Observacion
                };
                await _context.Bitacoras.AddAsync(b);
                n = await _context.SaveChangesAsync();

                var solicitud = await _context.Solicitudes.FindAsync(bitacora.IdSolicitud);
                solicitud.Estado = 3;//Cambiar estado de la solicitud a finalizada
                _context.Solicitudes.Update(solicitud);
                n = await _context.SaveChangesAsync();
                //Se realiza el commit de la transacción a la base de datos
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Muestra todos los datos de la bitácora.
        /// </summary>
        /// <param name="id">El ID de la bitácora.</param>
        /// <returns>Devuelve una vista parcial con los datos de la bitácora para cargar en una ventana emergente.</returns>
        public async Task<PartialViewResult> MasInformacionBitacora(int id = 0)
        {
            if (id == 0)
            {
                string mensaje = "Ocurrió un error al recibir los datos, inténtelo nuevamente o avise al administrador del sistema.";
                return PartialView("_PartialModalError", mensaje);
            }
            var bitacora = await _context.Bitacoras.Select(b => new vmBitacoraInfo()
            {
                Id = b.Id,
                FechaBitacora = b.Fecha,
                FechaSolicitado = b.IdSolicitudNavigation.FechaSolicitado,
                FechaSalida = b.IdSolicitudNavigation.FechaSalida,
                FechaLlegada = b.IdSolicitudNavigation.FechaLlegada,
                LitrosCombustible = (int)b.LitrosCombustible,
                Observacion = !string.IsNullOrEmpty(b.Observacion) ? b.Observacion : "Sin observación",
                Solicitante = b.IdSolicitudNavigation.IdSolicitanteNavigation.Nombre +" "+b.IdSolicitudNavigation.IdSolicitanteNavigation.Apellido,
                Conductor = b.IdConductorNavigation.IdUsuarioNavigation.Nombre +" "+ b.IdConductorNavigation.IdUsuarioNavigation.Apellido,
                Vehiculo = b.IdVehiculoNavigation.Patente +" - "+ b.IdVehiculoNavigation.Marca + " " + b.IdVehiculoNavigation.Modelo,
                KmInicial = b.IdKilometrajeNavigation.KilometrajeInicial,
                KmFinal = b.IdKilometrajeNavigation.KilometrajeFinal
            }).Where(b => b.Id == id).FirstOrDefaultAsync();
            if (bitacora == null)
            {
                string mensaje = "Ocurrió un error al obtener los datos, inténtelo nuevamente o avise al administrador del sistema.";
                return PartialView("_PartialModalError", mensaje);

            }
            return PartialView("_MasInformacionBitacora", bitacora);
        }

        /// <summary>
        /// Verifica si un vehículo cumple con el mantenimiento requerido basado en los kilómetros recorridos.
        /// </summary>
        /// <param name="id">El ID del vehículo a verificar.</param>
        /// <returns>Devuelve un valor booleano que indica si el vehículo cumple con el mantenimiento requerido.</returns>
        private bool VerificarMantenimientoVehiculo(int id = 0)
        {
            int kmRecorrido = 0;
            //Obtengo las mantenciones del vehículo
            var fichaMant = _context.Fichamantencions
                .Where(f => f.IdVehiculo == id)
                .ToList();
            //Obtengo el historial de kilometrajes del vehículo
            var kms = _context.Kilometrajes
                .Where(k => k.IdVehiculo == id)
                .ToList();
            //Obtengo el periodo asignado al vehículo
            var periodo = _context.Vehiculos.Include(v => v.IdPeriodoKilometrajeNavigation)
                .FirstOrDefault(v => v.Id == id)?.IdPeriodoKilometrajeNavigation.PeriodoKilometraje??0;
            //Verifico si existen mantenciones del vehículo
            if (!fichaMant.Any())
            {//Si no existe, sumar todo el kilometraje final e inicial y restar sus resultados.
                kmRecorrido = kms.Sum(k => k.KilometrajeFinal - k.KilometrajeInicial);
            }
            else
            {//Si existe, obtener el ultimo registro de mantención para obtener el kilometraje del vehículo
             //y este restarlo a la suma de los kilometrajes  
                var lastFM = fichaMant.LastOrDefault(fm => fm.IdVehiculo == id);
                kmRecorrido = lastFM.Kilometraje - kms.Where(k => k.FechaCreacion >= lastFM.FechaMantencion).Sum(k => k.KilometrajeFinal);
            }
            return periodo >= kmRecorrido; //Devuelve true si el periodo seleccionado para el vehículo es mayor que los kilómetros recorridos
        }
    }
}
