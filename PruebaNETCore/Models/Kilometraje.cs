using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class Kilometraje
{
    public int Id { get; set; }

    public decimal KilometrajeInicial { get; set; }

    public decimal KilometrajeFinal { get; set; }

    public int IdVehiculo { get; set; }

    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();

    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
