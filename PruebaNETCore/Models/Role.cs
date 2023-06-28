namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa los roles de los usuarios
/// </summary>
public partial class Role
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    public string Rol { get; set; }
    /// <summary>
    /// Relación uno es a mucho con la clase "RolesPermiso"
    /// </summary>
    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();
    /// <summary>
    /// Relación uno es a mucho con la clase "Usuariosrole" 
    /// </summary>
    public virtual ICollection<Usuariosrole> Usuariosroles { get; set; } = new List<Usuariosrole>();
}
