using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
public partial class Kilometraje
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int KilometrajeInicial { get; set; }
    public int KilometrajeFinal { get; set; }
    public int IdVehiculo { get; set; }
    public string Trial823 { get; set; }

    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();

    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
