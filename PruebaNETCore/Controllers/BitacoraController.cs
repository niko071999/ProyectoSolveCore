using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;

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
        [Authorize(Roles = "Adminstrador, Jefe, Conductor")]
        public async Task<IActionResult> VisualizarBitacora()
        {
            var bitacoras = await _context.Bitacoras.Include(b => b.IdSolicitudNavigation).Include(b => b.IdVehiculoNavigation)
                .Include(b => b.IdConductorNavigation.IdUsuarioNavigation).Include(b => b.IdKilometrajeNavigation)
                .ToListAsync();
            return View(bitacoras);
        }
        [Authorize(Roles = "Adminstrador, Conductor, Solicitador, Mantenedor de bitácora")]
        public async Task<IActionResult> AgregarEntradasBitacora(int id = 0)
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
            return RedirectToAction("MisSolicitudes", "Solicitudes");
        }
        [Authorize(Roles = "Adminstrador, Conductor, Solicitador, Mantenedor de bitácora")]
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
        [Authorize(Roles = "Adminstrador, Conductor, Solicitador, Mantenedor de bitácora")]
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
            using var transaction = await _context.Database.BeginTransactionAsync();
            var km = new Kilometraje()
            {
                IdVehiculo = bitacora.IdVehiculo,
                KilometrajeInicial = bitacora.KmInicialEntero,
                KilometrajeFinal = bitacora.KmFinalEntero
            };

            await _context.Kilometrajes.AddAsync(km);

            bool statusVehiculo = VerificarMantenimientoVehiculo(bitacora.IdVehiculo);
            if (!statusVehiculo)
            {
                var vehiculo = _context.Vehiculos.Find(bitacora.IdVehiculo);
                vehiculo.Estado = false;
                _context.Vehiculos.Update(vehiculo);
            }
            int n = await _context.SaveChangesAsync();
            if (n == 0)
            {
                return false;
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

        public bool VerificarMantenimientoVehiculo(int id = 0)
        {
            decimal kmRecorrido = 0.0m;
            var fichaMant = _context.FichaMantencions.ToList();
            var kms = _context.Kilometrajes.Where(k => k.IdVehiculo == id).ToList();
            var periodo = _context.Vehiculos.Include(v => v.IdPeriodoKilometrajeNavigation)
                .FirstOrDefault(v => v.Id == id).IdPeriodoKilometrajeNavigation.PeriodoKilometraje;
            if (!fichaMant.Any())
            {
                kmRecorrido = kms.Sum(k => k.KilometrajeInicial - k.KilometrajeFinal);
            }
            else
            {
                kmRecorrido = fichaMant.LastOrDefault(fm => fm.IdVehiculo == id).Kilometraje - kms.Sum(k => k.KilometrajeFinal);
            }
            return periodo <= kmRecorrido;
            //var porc = (kmRecorrido / periodo) * 100;
            //if (porc > 50)
            //{
            //    return "Precaucion";
            //}
        }
    }
}
