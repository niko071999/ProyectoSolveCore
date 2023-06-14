using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;

public partial class Vehiculo
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }    
    public string Patente { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    /// <summary>
    /// Año del vehículo
    /// </summary>
    public string Year { get; set; }
    /// <summary>
    /// Indica si el vehículo esta habilitado para las solicitudes
    /// </summary>
    public bool Estado { get; set; }
    /// <summary>
    /// Indica si el vehículo está eliminado
    /// </summary>
    public bool Eliminado { get; set; }
    /// <summary>
    /// Direccion de la imagen del vehículo
    /// </summary>
    public string DireccionImg { get; set; }
    /// <summary>
    /// Identificador unico del periodo del kilometraje
    /// </summary>
    public int IdPeriodoKilometraje { get; set; }
    /// <summary>
    /// Identificador unico de la categoría
    /// </summary>
    public int? IdCategoria { get; set; }
    /// <summary>
    /// Identificador unico del conductor
    /// </summary>
    public int? IdConductor { get; set; }
    public string Trial829 { get; set; }
    /// <summary>
    /// Relacion uno es a mucho de la bitácora
    /// </summary>
    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();
    /// <summary>
    /// Relacion uno es a mucho de la mantención
    /// </summary>
    public virtual ICollection<Fichamantencion> Fichamantencions { get; set; } = new List<Fichamantencion>();
    /// <summary>
    /// Relacion uno es a uno de la categoria
    /// </summary>
    public virtual Categoria IdCategoriaNavigation { get; set; }
    /// <summary>
    /// Relacion uno es a uno del conductor
    /// </summary>
    public virtual Conductore IdConductorNavigation { get; set; }
    /// <summary>
    /// Relacion uno es a uno del periodo de kilometraje
    /// </summary>
    public virtual Periodosmantenimiento IdPeriodoKilometrajeNavigation { get; set; }
    /// <summary>
    /// Relacion uno es a mucho del kilometraje
    /// </summary>
    public virtual ICollection<Kilometraje> Kilometrajes { get; set; } = new List<Kilometraje>();
    /// <summary>
    /// Relacion uno es a mucho de la solicitud
    /// </summary>
    public virtual ICollection<Solicitude> Solicitudes { get; set; } = new List<Solicitude>();
}
