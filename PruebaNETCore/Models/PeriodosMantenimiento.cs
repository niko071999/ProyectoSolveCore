using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class PeriodosMantenimiento
{
    public int Id { get; set; }

    public decimal PeriodoKilometraje { get; set; }

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
