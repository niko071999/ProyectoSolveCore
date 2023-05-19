using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class Departamento
{
    public int Id { get; set; }

    public string Departamento1 { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
