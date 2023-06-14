using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.NewModels;

/// <summary>
/// TRIAL
/// </summary>
public partial class Usuariosrole
{
    public int Id { get; set; }

    public int Idusuario { get; set; }

    public int Idrol { get; set; }

    public string Trial829 { get; set; }

    public virtual Role IdrolNavigation { get; set; }

    public virtual Usuario IdusuarioNavigation { get; set; }
}
