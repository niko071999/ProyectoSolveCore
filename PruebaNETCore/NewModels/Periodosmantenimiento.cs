using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.NewModels;

/// <summary>
/// TRIAL
/// </summary>
public partial class Periodosmantenimiento
{
    public int Id { get; set; }

    public int PeriodoKilometraje { get; set; }

    public string Trial823 { get; set; }

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
