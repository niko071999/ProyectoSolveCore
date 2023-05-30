using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
public partial class Fichamantencion
{
    public int Id { get; set; }
    public DateTime FechaMantencion { get; set; }
    public int Kilometraje { get; set; }
    public string Descripcion { get; set; }
    public int IdConductor { get; set; }
    public int IdVehiculo { get; set; }
    public string Trial823 { get; set; }

    public virtual Conductore IdConductorNavigation { get; set; }

    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
