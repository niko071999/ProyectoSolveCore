using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.NewModels;

/// <summary>
/// TRIAL
/// </summary>
public partial class Vehiculo
{
    public int Id { get; set; }

    public string Patente { get; set; }

    public string Marca { get; set; }

    public string Modelo { get; set; }

    public string Year { get; set; }

    public bool Estado { get; set; }

    public bool Eliminado { get; set; }

    public string DireccionImg { get; set; }

    public int IdPeriodoKilometraje { get; set; }

    public int? IdCategoria { get; set; }

    public int? IdConductor { get; set; }

    public string Trial829 { get; set; }

    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();

    public virtual ICollection<Fichamantencion> Fichamantencions { get; set; } = new List<Fichamantencion>();

    public virtual Categoria IdCategoriaNavigation { get; set; }

    public virtual Conductore IdConductorNavigation { get; set; }

    public virtual Periodosmantenimiento IdPeriodoKilometrajeNavigation { get; set; }

    public virtual ICollection<Kilometraje> Kilometrajes { get; set; } = new List<Kilometraje>();

    public virtual ICollection<Solicitude> Solicitudes { get; set; } = new List<Solicitude>();
}
