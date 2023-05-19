using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class Vehiculo
{
    public int Id { get; set; }

    public string Patente { get; set; }

    public string Marca { get; set; }

    public string Modelo { get; set; }

    public bool Estado { get; set; }

    public bool Eliminado { get; set; }

    public int IdPeriodoKilometraje { get; set; }

    public string DireccionImg { get; set; }

    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();

    public virtual ICollection<FichaMantencion> FichaMantencions { get; set; } = new List<FichaMantencion>();

    public virtual PeriodosMantenimiento IdPeriodoKilometrajeNavigation { get; set; }

    public virtual ICollection<Kilometraje> Kilometrajes { get; set; } = new List<Kilometraje>();

    public virtual ICollection<Solicitude> Solicitudes { get; set; } = new List<Solicitude>();
}
