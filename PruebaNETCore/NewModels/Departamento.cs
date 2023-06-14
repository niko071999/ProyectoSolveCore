using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.NewModels;

/// <summary>
/// TRIAL
/// </summary>
public partial class Departamento
{
    public int Id { get; set; }

    public string Departamento1 { get; set; }

    public string Trial820 { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
