using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;

namespace ProyectoSolveCore.Filters
{
    /// <summary>
    /// Filtro de autorización para verificar los permisos del usuario.
    /// </summary>
    public class AutorizarFilter : IAsyncAuthorizationFilter
    {
        private readonly PermisoRequerimiento _permisoRequerimiento;
        private readonly ModelData _context;
        /// <summary>
        /// El constructor de la clase AutorizarFilter.
        /// </summary>
        /// <param name="permisoRequerimiento">El requerimiento de permiso que se va a verificar.</param>
        /// <param name="context">El contexto de datos del modelo que contiene las tablas de la base de datos.</param>
        public AutorizarFilter(PermisoRequerimiento permisoRequerimiento, ModelData context)
        {
            _permisoRequerimiento = permisoRequerimiento;
            _context = context;
        }
        /// <summary>
        /// Acción que se ejecuta cuando se aplica el filtro de autorización.
        /// </summary>
        /// <param name="context">El contexto del filtro de autorización que contiene la información del usuario y la acción.</param>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var roles = await _context.RolesPermisos.Where(rp => rp.IdPermiso == _permisoRequerimiento.PermisoId)
                    .Select(rp => rp.IdRolNavigation.Rol)
                    .ToListAsync();

            if (roles.Any(r => context.HttpContext.User.IsInRole(r)))
            {
                // Si el usuario tiene el permiso, se permite la ejecución de la acción.
                return;
            }
            // Si el usuario no tiene el permiso, se devuelve un resultado de prohibición.
            context.Result = new ForbidResult();
        }
    }
}
