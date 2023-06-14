namespace ProyectoSolveCore.Models
{
    /// <summary>
    /// Representa el requerimiento de la clase Permiso para ocuparlo 
    /// en la clase filtros de autorizacion de los usuarios
    /// </summary>
    public class PermisoRequerimiento
    {
        /// <summary>
        /// Identificador unico
        /// </summary>
        public int PermisoId { get; set; }
        //Constructor que recibe el identificador unico del permiso
        public PermisoRequerimiento(int permisoId)
        {
            PermisoId = permisoId;
        }
    }
}
