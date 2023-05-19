using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class UsuariosRole
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public virtual Role IdRolNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; }
}
