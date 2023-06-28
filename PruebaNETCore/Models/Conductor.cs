namespace ProyectoSolveCore.Models;
public partial class Conductor
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Numero de la póliza
    /// </summary>
    public int NumeroPoliza { get; set; }
    /// <summary>
    /// Fecha de emisión de la póliza
    /// </summary>
    public DateTime FechaEmision { get; set; }
    /// <summary>
    /// Fecha de vencimiento de la póliza
    /// </summary>
    public DateTime FechaVencimiento { get; set; }
    /// <summary>
    /// Indica si el conductor está disponible
    /// </summary>
    public bool Estado { get; set; }
    /// <summary>
    /// Indica si el conductor esta eliminado
    /// </summary>
    public bool Eliminado { get; set; }
    /// <summary>
    /// Identificador único del usuario
    /// </summary>
    public int IdUsuario { get; set; }
    /// <summary>
    /// Relación uno es a muchos con la bitácora
    /// </summary>
    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();
    /// <summary>
    /// Relación uno es a muchos con la mantención
    /// </summary>
    public virtual ICollection<Fichamantencion> Fichamantencions { get; set; } = new List<Fichamantencion>();
    /// <summary>
    /// Relación uno es a uno con el usuario
    /// </summary>
    public virtual Usuario IdUsuarioNavigation { get; set; }
    /// <summary>
    /// Relación uno es a muchos con la solicitud
    /// </summary>
    public virtual ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
    /// <summary>
    /// Relación uno es a muchos con el vehículo
    /// </summary>
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
