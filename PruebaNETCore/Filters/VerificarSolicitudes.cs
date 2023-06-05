using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using System.Globalization;

namespace ProyectoSolveCore.Filters
{
    public class VerificarSolicitudes : IActionFilter
    {
        private readonly ModelData db = new ModelData();
        public async void OnActionExecuted(ActionExecutedContext context)
        {
            
            var hoy = DateTime.Now;
            var s = await db.Solicitudes.Where(s => s.Estado == 0 && s.FechaSalida < hoy).ToListAsync();
            if (s.Any())
            {
                s.ForEach(s => s.Estado = 2);
                var aprobaciones = s.Select(s => new Aprobacione()
                {
                    Estado = false,
                    Fecha = hoy,
                    IdJefe = 1,
                    IdSolicitud = s.Id,
                    Motivo = "Expiro la fecha de salida de la solicitud"
                }).ToList();
                await db.AddRangeAsync(aprobaciones);
                await db.SaveChangesAsync();
            }
        }

        private DateTime GenerarFecha(DateTime now)
        {
            var hoystr = now.ToString("dd-MM-yyyy HH:mm:ss");
            return DateTime.ParseExact(hoystr, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("Se ejecuto el codigo");
        }
    }
}
