using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Rol { get; set; }

    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();

    public virtual ICollection<UsuariosRole> UsuariosRoles { get; set; } = new List<UsuariosRole>();
}
