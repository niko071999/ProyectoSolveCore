namespace ProyectoSolveCore.Models;
public partial class Vehiculo
{
    /// <summary>
    /// Identificador único
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
    /// Dirección de la imagen del vehículo
    /// </summary>
    public string DireccionImg { get; set; }
    /// <summary>
    /// Identificador único del periodo del kilometraje
    /// </summary>
    public int IdPeriodoKilometraje { get; set; }
    /// <summary>
    /// Identificador único de la categoría
    /// </summary>
    public int? IdCategoria { get; set; }
    /// <summary>
    /// Identificador único del conductor
    /// </summary>
    public int? IdConductor { get; set; }
    public string Trial829 { get; set; }
    /// <summary>
    /// Relación uno es a mucho de la bitácora
    /// </summary>
    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();
    /// <summary>
    /// Relación uno es a mucho de la mantención
    /// </summary>
    public virtual ICollection<Fichamantencion> Fichamantencions { get; set; } = new List<Fichamantencion>();
    /// <summary>
    /// Relación uno es a uno de la categoría
    /// </summary>
    public virtual Categoria IdCategoriaNavigation { get; set; }
    /// <summary>
    /// Relación uno es a uno del conductor
    /// </summary>
    public virtual Conductor IdConductorNavigation { get; set; }
    /// <summary>
    /// Relación uno es a uno del periodo de kilometraje
    /// </summary>
    public virtual Periodosmantenimiento IdPeriodoKilometrajeNavigation { get; set; }
    /// <summary>
    /// Relación uno es a mucho del kilometraje
    /// </summary>
    public virtual ICollection<Kilometraje> Kilometrajes { get; set; } = new List<Kilometraje>();
    /// <summary>
    /// Relación uno es a mucho de la solicitud
    /// </summary>
    public virtual ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
}
