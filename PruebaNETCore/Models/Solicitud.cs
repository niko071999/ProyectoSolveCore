namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa la entidad solicitud del vehículo
/// </summary>
public partial class Solicitud
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    public DateTime FechaSolicitado { get; set; }
    public DateTime FechaSalida { get; set; }
    public DateTime FechaLlegada { get; set; }
    /// <summary>
    /// Números de pasajeros que van en el vehículo
    /// </summary>
    public int NumeroPasajeros { get; set; }
    /// <summary>
    /// Nombre de los pasajeros que van en el vehículo
    /// </summary>
    public string Pasajeros { get; set; }
    /// <summary>
    /// Localidad, o dirección a la que va el vehículo
    /// </summary>
    public string Destino { get; set; }
    /// <summary>
    /// Descripción del motivo del viaje
    /// </summary>
    public string Motivo { get; set; }
    /// <summary>
    /// Indica el estado del vehículo
    /// </summary>
    /// <remarks>
    /// 0 = Pendiente, 1 = Aprobada, 2 = Rechazada, 3 = Finalizada
    /// </remarks>
    public int Estado { get; set; }
    /// <summary>
    /// Identificador único del usuario solicitante
    /// </summary>
    public int IdSolicitante { get; set; }
    /// <summary>
    /// Identificador único del vehículo
    /// </summary>
    public int IdVehiculo { get; set; }
    /// <summary>
    /// Identificador único del conductor
    /// </summary>
    public int? IdConductor { get; set; }
    public string Trial826 { get; set; }
    /// <summary>
    /// Relación uno es a mucho de la aprobación
    /// </summary>
    public virtual ICollection<Aprobacion> Aprobaciones { get; set; } = new List<Aprobacion>();
    /// <summary>
    /// Relación uno es a mucho de la bitácora
    /// </summary>
    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();
    /// <summary>
    /// Relación uno es a uno del conductor
    /// </summary>
    public virtual Conductor IdConductorNavigation { get; set; }
    /// <summary>
    /// Relación uno es a uno del usuario solicitante
    /// </summary>
    public virtual Usuario IdSolicitanteNavigation { get; set; }
    /// <summary>
    /// Relación uno es a uno del vehículo
    /// </summary>
    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
