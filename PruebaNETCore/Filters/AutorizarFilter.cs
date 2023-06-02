﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;

namespace ProyectoSolveCore.Filters
{
    public class AutorizarFilter : IAsyncAuthorizationFilter
    {
        private readonly PermisoRequerimiento _permisoRequerimiento;
        private readonly ModelData _context;
        public AutorizarFilter(PermisoRequerimiento permisoRequerimiento, ModelData context)
        {
            _permisoRequerimiento = permisoRequerimiento;
            _context = context;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var roles = await _context.RolesPermisos.Where(rp => rp.IdPermiso == _permisoRequerimiento.PermisoId)
                    .Select(rp => rp.IdRolNavigation.Rol)
                    .ToListAsync();

            if (roles.Any(r => context.HttpContext.User.IsInRole(r)))
            {
                return;
            }
            context.Result = new ForbidResult();
        }
    }
}
