using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class FichaMantencion
{
    public int Id { get; set; }

    public DateTime FechaMantencion { get; set; }

    public decimal Kilometraje { get; set; }

    public string Descripcion { get; set; }

    public int IdConductor { get; set; }

    public int IdVehiculo { get; set; }

    public virtual Conductore IdConductorNavigation { get; set; }

    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
