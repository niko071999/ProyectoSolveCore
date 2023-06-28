using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using System.Globalization;

namespace ProyectoSolveCore.Filters
{
    /// <summary>
    /// Filtro que verifica los estados de las solicitudes creadas.
    /// </summary>
    public class VerificarSolicitudes : IActionFilter
    {
        private readonly ModelData db = new();
        /// <summary>
        /// Método que se ejecuta después de que se completa la acción.
        /// </summary>
        /// <param name="context">El contexto del filtro de acción que contiene la información de la acción y el resultado.</param>
        public async void OnActionExecuted(ActionExecutedContext context)
        {
            var hoy = DateTime.Now;
            var s = await db.Solicitudes.Where(s => s.Estado == 0 && s.FechaSalida < hoy).ToListAsync();
            if (s.Any())
            {
                s.ForEach(s => s.Estado = 2);
                var aprobaciones = s.Select(s => new Aprobacion()
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
        /// <summary>
        /// Método que se ejecuta antes de que se inicie la acción.
        /// </summary>
        /// <param name="context">El contexto del filtro de acción que contiene la información de la acción.</param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
