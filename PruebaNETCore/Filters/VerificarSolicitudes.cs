using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;

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
                await db.SaveChangesAsync();
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("Se ejecuto el codigo");
        }
    }
}
