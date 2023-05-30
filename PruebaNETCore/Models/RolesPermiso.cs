using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
public partial class RolesPermiso
{
    public int Id { get; set; }
    public int IdRol { get; set; }
    public int IdPermiso { get; set; }
    public string Trial826 { get; set; }

    public virtual Permiso IdPermisoNavigation { get; set; }

    public virtual Role IdRolNavigation { get; set; }
}
