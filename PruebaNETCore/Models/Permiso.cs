namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa los permisos o acciones dentro del sistema
/// </summary>
public partial class Permiso
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Nombre del permiso
    /// </summary>
    public string Permiso1 { get; set; }
    /// <summary>
    /// Relación uno es a mucho con la clase "RolesPermiso"
    /// </summary>
    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();
}
