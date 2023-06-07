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
    [Authorize]
    public class BitacoraController : Controller
    {
        private readonly ModelData _context;
        private readonly IMemoryCache _cache;
        public BitacoraController(ModelData context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        //[Authorize(Roles = "Adminstrador, Jefe, Conductor, Mantenedor de bitácora")]
        [Autorizar(19)]
        public async Task<IActionResult> VisualizarBitacora()
        {
            var bitacoras = await _context.Bitacoras.Include(b => b.IdSolicitudNavigation).Include(b => b.IdVehiculoNavigation)
                .Include(b => b.IdConductorNavigation.IdUsuarioNavigation).Include(b => b.IdKilometrajeNavigation)
                .ToListAsync();
            return View(bitacoras);
        }
        [Autorizar(1)]
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
        [Autorizar(1)]
        [HttpPost]
        public async Task<IActionResult> AgregarEntradasBitacora(vmBitacora bitacora) //Esta accion permite la insercion de varias entradas a la bitacora.
        {
            if (bitacora == null)
            {
                return RedirectToAction("MisSolicitudes","Solicitudes");
            }
            var checkSol = await _context.Bitacoras.AnyAsync(b => b.IdSolicitud == bitacora.IdSolicitud);
            if (checkSol) //Verifica si existe la solicitud ya ingresada en la bitacora
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
        [Autorizar(1)]
        [HttpPost]
        public async Task<IActionResult> AgregarEntradaBitacora(vmBitacora bitacora)
        {

            if (bitacora == null)
            {
                return RedirectToAction("VisualizarBitacora", "Bitacora");
            }
            var checkSol = await _context.Bitacoras.AnyAsync(b => b.IdSolicitud == bitacora.IdSolicitud);
            if (checkSol) //Verifica si existe la solicitud ya ingresada en la bitacora
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
                return RedirectToAction("VisualizarBitacora", "Bitacora");
            }
            catch (Exception)
            {
                return View(bitacora);
            }
        }

        private void EliminarMemoriaCache()
        {
            if (_cache.TryGetValue("NumeroSolicitudes", out int _))
            {
                _cache.Remove("NumeroSolicitudes");
            }
        }

        private async Task<bool> AgregarEntrada(vmBitacora bitacora)
        {
            try
            {
                if (bitacora.KmInicialEntero > bitacora.KmFinalEntero) return false;

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
                bool statusVehiculo = VerificarMantenimientoVehiculo(bitacora.IdVehiculo);
                int n = 0;
                if (!statusVehiculo)
                {
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

                var solicitud = await _context.Solicitudes.FindAsync(bitacora.IdSolicitud);
                solicitud.Estado = 3;//Cambiar estado de la solicitud a finalizada
                _context.Solicitudes.Update(solicitud);
                n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    return false;
                }
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static DateTime GenerarFecha(DateTime now)
        {
            var hoystr = now.ToString("dd-MM-yyyy HH:mm:ss");
            return DateTime.ParseExact(hoystr, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public async Task<PartialViewResult> MasInformacionBitacora(int id = 0)
        {
            if (id == 0)
            {
                string mensaje = "Ocurrio un error al recibir los datos, intentelo nuevamente o avise al administrador del sistema.";
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
                string mensaje = "Ocurrio un error al obtener los datos, intentelo nuevamente o avise al administrador del sistema.";
                return PartialView("_PartialModalError", mensaje);

            }
            return PartialView("_MasInformacionBitacora", bitacora);
        }

        public bool VerificarMantenimientoVehiculo(int id = 0)
        {
            int kmRecorrido = 0;
            var fichaMant = _context.Fichamantencions
                .Where(f => f.IdVehiculo == id)
                .ToList();
            var kms = _context.Kilometrajes
                .Where(k => k.IdVehiculo == id)
                .ToList();
            var periodo = _context.Vehiculos.Include(v => v.IdPeriodoKilometrajeNavigation)
                .FirstOrDefault(v => v.Id == id)?.IdPeriodoKilometrajeNavigation.PeriodoKilometraje??0;
            if (!fichaMant.Any())
            {
                kmRecorrido = kms.Sum(k => k.KilometrajeFinal - k.KilometrajeInicial);
            }
            else
            {
                var lastFM = fichaMant.LastOrDefault(fm => fm.IdVehiculo == id);
                kmRecorrido = fichaMant.LastOrDefault(fm => fm.IdVehiculo == id) != null 
                    ? lastFM.Kilometraje - kms.Where(k => k.FechaCreacion >= lastFM.FechaMantencion).Sum(k => k.KilometrajeFinal) : 0;
            }
            return periodo >= kmRecorrido; //Devuelve true si el periodo seleccionado para el vehiculo es mayor que los kilometros recorridos
        }
    }
}
