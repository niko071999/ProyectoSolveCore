using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
public partial class Periodosmantenimiento
{
    public int Id { get; set; }
    public int PeriodoKilometraje { get; set; }
    public string Trial823 { get; set; }

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
