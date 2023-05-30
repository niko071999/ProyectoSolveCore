using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
public partial class Role
{
    public int Id { get; set; }
    public string Rol { get; set; }
    public string Trial823 { get; set; }

    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();

    public virtual ICollection<Usuariosrole> Usuariosroles { get; set; } = new List<Usuariosrole>();
}
