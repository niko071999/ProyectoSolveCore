using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;

namespace ProyectoSolveCore.Controllers
{
    public class BitacoraController : Controller
    {
        private readonly ModelData _context;
        public BitacoraController(ModelData context)
        {
            _context = context;
        }
        public async Task<IActionResult> VisualizarBitacora()
        {
            var bitacoras = await _context.Bitacoras.Include(b => b.IdSolicitudNavigation).Include(b => b.IdVehiculoNavigation)
                .Include(b => b.IdConductorNavigation.IdUsuarioNavigation).Include(b => b.IdKilometrajeNavigation)
                .ToListAsync();
            return View(bitacoras);
        }

        public async Task<IActionResult> AgregarEntradaBitacora(int id = 0)
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

            return View(bitacora);
        }
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
                return RedirectToAction("HubServiceSolicitudes", "Solicitud");
            }
            try
            {
                bool valid = await AgregarEntrada(bitacora);

                if (!valid)
                {
                    return View(bitacora);
                }
                return RedirectToAction("HubServiceSolicitudes", "Solicitud");
            }
            catch (Exception ex)
            {
                return View(bitacora);
            }
        }
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
                return RedirectToAction("HubServiceSolicitudes", "Solicitud");
            }
            try
            {
                bool valid = await AgregarEntrada(bitacora);

                if (!valid)
                {
                    return View(bitacora);
                }
                return RedirectToAction("VisualizarBitacora", "Bitacora");
            }
            catch (Exception)
            {
                return View(bitacora);
            }
        }
        private async Task<bool> AgregarEntrada(vmBitacora bitacora)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            string kmInicialjeStr = $"{bitacora.KmInicialEntero}.{bitacora.KmInicialDecimal}";
            string kmFinaljeStr = $"{bitacora.KmFinalEntero}.{bitacora.KmFinalDecimal}";
            if (!decimal.TryParse(kmInicialjeStr, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal kilometrajeInicial))
            {
                return false;
            }
            if (!decimal.TryParse(kmFinaljeStr, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal kilometrajeFinal))
            {
                return false;
            }
            var km = new Kilometraje()
            {
                IdVehiculo = bitacora.IdVehiculo,
                KilometrajeInicial = kilometrajeInicial,
                KilometrajeFinal = kilometrajeFinal
            };

            await _context.Kilometrajes.AddAsync(km);

            string statusVehiculo = VerificarMantenimientoVehiculo(bitacora.IdVehiculo);
            if (statusVehiculo.Equals("Deshabilitado"))
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
                IdConductor = bitacora.IdConductor,
                IdKilometraje = km.Id,
                IdSolicitud = bitacora.IdSolicitud,
                IdVehiculo = bitacora.IdVehiculo,
                Observacion = bitacora.Observacion
            };

            await _context.Bitacoras.AddAsync(b);

            var solicitud = await _context.Solicitudes.FindAsync(bitacora.IdSolicitud);
            solicitud.Estado = 3;//Solicitud estado finalizada
            _context.Solicitudes.Update(solicitud);
            n = await _context.SaveChangesAsync();
            if (n == 0)
            {
                return false;
            }
            await transaction.CommitAsync();
            return true;
        }

        public string VerificarMantenimientoVehiculo(int id = 0)
        {
            decimal kmRecorrido = 0.0m;
            var fichaMant = _context.FichaMantencions.ToList();
            var kms = _context.Kilometrajes.Where(k => k.IdVehiculo == id).ToList();
            var periodo = _context.Vehiculos.FirstOrDefault(v => v.Id == id).IdPeriodoKilometrajeNavigation.PeriodoKilometraje;
            if (!fichaMant.Any())
            {
                kmRecorrido = kms.Sum(k => k.KilometrajeInicial - k.KilometrajeFinal);
            }
            else
            {
                kmRecorrido = fichaMant.LastOrDefault(fm => fm.IdVehiculo == id).Kilometraje - kms.Sum(k => k.KilometrajeFinal);
            }
            if (periodo <= kmRecorrido)
            {
                return "Deshabilitado";
            }
            return "Habilitado";
            //var porc = (kmRecorrido / periodo) * 100;
            //if (porc > 50)
            //{
            //    return "Precaucion";
            //}
        }
    }
}
