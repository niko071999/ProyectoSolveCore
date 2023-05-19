using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;

namespace ProyectoSolveCore.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ModelData _context;
        public ReporteController(ModelData context)
        {
            _context = context;
        }
        public IActionResult CantidadSolicitudesMensuales()
        {
            try
            {
                //rcsDic = Reporte de cantidad de solicitudes Diccionario
                Dictionary<DateTime, CountSolicitudes> rcsDic = new();
                var FirsDayYear = new DateTime(DateTime.Now.Year, 1, 1);
                var LastDayYear = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
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
    }
}
