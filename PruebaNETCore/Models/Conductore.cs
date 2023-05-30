using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class Conductore
{
    public int Id { get; set; }

    public int NumeroPoliza { get; set; }

    public DateTime FechaEmision { get; set; }

    public DateTime FechaVencimiento { get; set; }

    public bool Estado { get; set; }

    public bool Eliminado { get; set; }

    public int IdUsuario { get; set; }

    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();

    public virtual ICollection<FichaMantencion> FichaMantencions { get; set; } = new List<FichaMantencion>();

    public virtual Usuario IdUsuarioNavigation { get; set; }

    public virtual ICollection<Solicitude> Solicitudes { get; set; } = new List<Solicitude>();

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
