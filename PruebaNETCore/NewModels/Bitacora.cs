using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.NewModels;

/// <summary>
/// TRIAL
/// </summary>
public partial class Bitacora
{
    public long Id { get; set; }

    public long Folio { get; set; }

    public DateTime Fecha { get; set; }

    public int? LitrosCombustible { get; set; }

    public string Observacion { get; set; }

    public int IdVehiculo { get; set; }

    public int IdConductor { get; set; }

    public int IdSolicitud { get; set; }

    public int IdKilometraje { get; set; }

    public string Trial820 { get; set; }

    public virtual Conductore IdConductorNavigation { get; set; }

    public virtual Kilometraje IdKilometrajeNavigation { get; set; }

    public virtual Solicitude IdSolicitudNavigation { get; set; }

    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
