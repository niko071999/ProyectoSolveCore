namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa la tabla intermedia entre Rol y Permiso. 
/// Contiene los identificadores únicos de ambos
/// </summary>
public partial class RolesPermiso
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Identificador único del rol
    /// </summary>
    public int IdRol { get; set; }
    /// <summary>
    /// Identificador único del permiso
    /// </summary>
    public int IdPermiso { get; set; }
    /// <summary>
    /// Relación uno es a uno del permiso
    /// </summary>
    public virtual Permiso IdPermisoNavigation { get; set; }
    /// <summary>
    /// Relación uno es a uno del rol
    /// </summary>
    public virtual Role IdRolNavigation { get; set; }
}
