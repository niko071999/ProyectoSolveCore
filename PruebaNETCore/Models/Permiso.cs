namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa los permisos o acciones dentro del sistema
/// </summary>
public partial class Permiso
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Nombre del permiso
    /// </summary>
    public string Permiso1 { get; set; }
    public string Trial823 { get; set; }
    /// <summary>
    /// Relacion uno es a mucho con la clase "RolesPermiso"
    /// </summary>
    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();
}
