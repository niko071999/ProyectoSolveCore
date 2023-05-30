using ProyectoSolveCore.Models;
using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
public partial class Solicitude
{
    public int Id { get; set; }
    public DateTime FechaSolicitado { get; set; }
    public DateTime FechaSalida { get; set; }
    public DateTime FechaLlegada { get; set; }
    public int NumeroPasajeros { get; set; }
    public string Pasajeros { get; set; }
    public string Destino { get; set; }
    public string Motivo { get; set; }
    public int Estado { get; set; }
    public int IdSolicitante { get; set; }
    public int IdVehiculo { get; set; }
    public int? IdConductor { get; set; }
    public string Trial826 { get; set; }

    public virtual ICollection<Aprobacione> Aprobaciones { get; set; } = new List<Aprobacione>();

    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();

    public virtual Conductore IdConductorNavigation { get; set; }

    public virtual Usuario IdSolicitanteNavigation { get; set; }

    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
