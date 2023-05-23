using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class Categoria
{
    public int Id { get; set; }

    public string Categoria1 { get; set; }

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
