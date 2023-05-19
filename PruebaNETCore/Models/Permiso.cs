using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class Permiso
{
    public int Id { get; set; }

    public string Permiso1 { get; set; }

    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();
}
