using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa los roles de los usuarios
/// </summary>
public partial class Role
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }
    public string Rol { get; set; }
    public string Trial823 { get; set; }
    /// <summary>
    /// Relacion uno es a mucho con la clase "RolesPermiso"
    /// </summary>
    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();
    /// <summary>
    /// Relacion uno es a mucho con la clase "Usuariosrole" 
    /// </summary>
    public virtual ICollection<Usuariosrole> Usuariosroles { get; set; } = new List<Usuariosrole>();
}
