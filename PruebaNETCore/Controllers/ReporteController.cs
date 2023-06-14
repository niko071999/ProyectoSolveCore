using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ClasesReportes;
using ProyectoSolveCore.Models.ViewModels;
using System.Globalization;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con los reportes.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver y crear los reportes.
    /// </remarks>
    public class ReporteController : Controller
    {
        private readonly ModelData _context;
        public ReporteController(ModelData context)
        {
            _context = context;
        }
        [Autorizar(4)]
        public async Task<IActionResult> CantidadViajesFuncionario()
        {
            var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation.IdDepartamentoNavigation)
                .Where(s => s.IdSolicitante > 2 && s.Estado == 1 || s.Estado == 3)
                .ToListAsync();
            Dictionary<string, CountViajesFuncionarios> cvf = new();
            foreach (var s in solicitudes)
            {
                string nombre = $"{s.IdSolicitanteNavigation.Nombre} {s.IdSolicitanteNavigation.Apellido}";
                if (!cvf.ContainsKey(nombre))
                {
                    cvf.Add(nombre, new CountViajesFuncionarios
                    {
                        Nombre = nombre,
                        Departamento = s.IdSolicitanteNavigation.IdDepartamentoNavigation.Departamento1,
                        NumeroViaje = solicitudes.Where(x => x.IdSolicitante == s.IdSolicitante).ToList().Count
                    });
                }
            }
            return View(cvf.Values.OrderBy(x => x.Nombre).ToList());
        }
        [Autorizar(4)]
        public async Task<IActionResult> CantidadViajesConductores()
        {
            var solicitudes = await _context.Solicitudes.Include(s => s.IdConductorNavigation.IdUsuarioNavigation.IdDepartamentoNavigation)
                .Where(s => s.IdSolicitante > 2 && s.Estado == 1 || s.Estado == 3)
                .ToListAsync();
            var kilometrajes = await _context.Kilometrajes.Include(k => k.IdVehiculoNavigation.IdConductorNavigation)
                .ToListAsync();

            Dictionary<string, CountViajesConductores> cvc = new();
            foreach (var s in solicitudes)
            {
                string nombre = $"{s.IdConductorNavigation.IdUsuarioNavigation.Nombre} {s.IdConductorNavigation.IdUsuarioNavigation.Apellido}";
                if (!cvc.ContainsKey(nombre))
                {
                    var kmsUser = kilometrajes.Where(k => k.IdVehiculoNavigation.IdConductor.HasValue
                            && k.IdVehiculoNavigation.IdConductor == s.IdConductor).ToList();
                    var km = kmsUser.FirstOrDefault();
                    cvc.Add(nombre, new CountViajesConductores
                    {
                        Nombre = nombre,
                        Departamento = s.IdConductorNavigation.IdUsuarioNavigation.IdDepartamentoNavigation.Departamento1,
                        NumeroViaje = solicitudes.Where(x => x.IdConductor == s.IdConductor).ToList().Count,
                        KilometrajesTotales = kmsUser.Sum(k => km != null ? km.KilometrajeInicial - (k.KilometrajeFinal - k.KilometrajeInicial): 0)
                    });
                }
            }
            return View(cvc.Values.OrderBy(x => x.Nombre).ToList());
        }
        [Autorizar(4)]
        public IActionResult CantidadSolicitudesMensuales()
        {
            try
            {
                //rcsDic = Reporte de cantidad de solicitudes Diccionario
                Dictionary<DateTime, CountSolicitudes> rcsDic = new();
                var hoy = DateTime.Now;

                var FirsDayYear = new DateTime(hoy.Year, 1, 1);
                var LastDayYear = new DateTime(hoy.Year, hoy.Month, 1)
                    .AddMonths(-1).AddDays(-1);
                var solicitudes = _context.Solicitudes.OrderBy(s => s.FechaSolicitado)
                    .Where(s => s.FechaSolicitado >= FirsDayYear && s.FechaSolicitado <= LastDayYear)
                    .ToArray();
                int sol = 0, sp = 0, sa = 0, sr = 0, sf = 0; //Contadores de solicitudes

                foreach (var s in solicitudes)
                {
                    sol++;
                    if (s.Estado == 0) sp++;
                    if (s.Estado == 1) sa++;
                    if (s.Estado == 2) sr++;
                    if (s.Estado == 3) sf++;

                    if (rcsDic.ContainsKey(s.FechaSolicitado))
                    {
                        rcsDic[s.FechaSolicitado.Date] = new CountSolicitudes()
                        {
                            Total = sol,
                            TotalPendientes = sp,
                            TotalAprobadas = sa,
                            TotalRechazadas = sr,
                            TotalFinalizadas = sf
                        };
                    }
                    else
                    {
                        rcsDic.Add(s.FechaSolicitado.Date, new CountSolicitudes()
                        {
                            Total = 1,
                            TotalPendientes = 1,
                            TotalAprobadas = 1,
                            TotalRechazadas = 1,
                            TotalFinalizadas = 1
                        });
                    }
                }
                List<vmCantidadSolicitudes> rcsList = new();
                foreach (var s in rcsDic)
                {
                    decimal por_Pendiente = (s.Value.TotalPendientes / s.Value.Total) * 100;
                    decimal por_Aprobada = (s.Value.TotalAprobadas / s.Value.Total) * 100;
                    decimal por_Rechazada = (s.Value.TotalRechazadas / s.Value.Total) * 100;
                    decimal por_Finalizada = (s.Value.TotalFinalizadas / s.Value.Total) * 100;
                    vmCantidadSolicitudes cs = new()
                    {
                        Mes = s.Key,
                        TotalSolicitudes = s.Value.Total,
                        TotalSolicitudesPend = s.Value.TotalPendientes,
                        TotalSolicitudesAprob = s.Value.TotalAprobadas,
                        TotalSolicitudesRech = s.Value.TotalRechazadas,
                        TotalSolicitudesFin = s.Value.TotalFinalizadas,
                        PorcentajePendiente = por_Pendiente,
                        PorcentajeAprobado = por_Aprobada,
                        PorcentajeRechazado = por_Rechazada,
                        PorcentajeFinalizado = por_Finalizada
                    };
                    rcsList.Add(cs);
                }
                return View(rcsList);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        private DateTime GenerarFecha(DateTime now)
        {
            var hoystr = now.ToString("dd-MM-yyyy HH:mm:ss");
            return DateTime.ParseExact(hoystr, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
