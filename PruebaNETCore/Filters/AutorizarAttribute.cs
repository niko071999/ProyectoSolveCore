using Microsoft.AspNetCore.Mvc;
using ProyectoSolveCore.Models;

namespace ProyectoSolveCore.Filters
{
    public class AutorizarAttribute : TypeFilterAttribute
    {
        public AutorizarAttribute(int permisoId) : base(typeof(AutorizarFilter))
        {
            Arguments = new object[] { new PermisoRequerimiento(permisoId) };
        }
    }
}
