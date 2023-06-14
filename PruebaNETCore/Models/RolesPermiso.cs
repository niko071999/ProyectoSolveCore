namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa la tabla intermedia entre Rol y Permiso. 
/// Contiene los identificadores unicos de ambos
/// </summary>
public partial class RolesPermiso
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Identificador unico del rol
    /// </summary>
    public int IdRol { get; set; }
    /// <summary>
    /// Identificador unico del permiso
    /// </summary>
    public int IdPermiso { get; set; }
    public string Trial826 { get; set; }
    /// <summary>
    /// Relacion uno es a uno del permiso
    /// </summary>
    public virtual Permiso IdPermisoNavigation { get; set; }
    /// <summary>
    /// Relacion uno es a uno del rol
    /// </summary>
    public virtual Role IdRolNavigation { get; set; }
}
