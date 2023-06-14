using Microsoft.AspNetCore.Mvc;
using ProyectoSolveCore.Models;

namespace ProyectoSolveCore.Filters
{
    /// <summary>
    /// Atributo utilizado para autorizar el acceso a un recurso o acción basado en un permiso.
    /// </summary>
    public class AutorizarAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Crea una nueva instancia de la clase AutorizarAttribute con el ID de permiso especificado.
        /// </summary>
        /// <param name="permisoId">El ID del permiso o accion requerido para acceder a la vista</param>
        public AutorizarAttribute(int permisoId) : base(typeof(AutorizarFilter))
        {
            Arguments = new object[] { new PermisoRequerimiento(permisoId) };
        }
    }
}
