using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.NewModels;

/// <summary>
/// TRIAL
/// </summary>
public partial class Conductore
{
    public int Id { get; set; }

    public int NumeroPoliza { get; set; }

    public DateOnly FechaEmision { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public bool Estado { get; set; }

    public bool Eliminado { get; set; }

    public int IdUsuario { get; set; }

    public string Trial820 { get; set; }

    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();

    public virtual ICollection<Fichamantencion> Fichamantencions { get; set; } = new List<Fichamantencion>();

    public virtual Usuario IdUsuarioNavigation { get; set; }

    public virtual ICollection<Solicitude> Solicitudes { get; set; } = new List<Solicitude>();

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
