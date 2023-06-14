namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa las solicitudes de los vehículos
/// </summary>
public partial class Solicitude
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }
    public DateTime FechaSolicitado { get; set; }
    public DateTime FechaSalida { get; set; }
    public DateTime FechaLlegada { get; set; }
    /// <summary>
    /// Numeros de pasajeros que van en el vehículo
    /// </summary>
    public int NumeroPasajeros { get; set; }
    /// <summary>
    /// Nombre de los pasajeros que van en el vehículo
    /// </summary>
    public string Pasajeros { get; set; }
    /// <summary>
    /// Localidad, o direccion a la que va el vehículo
    /// </summary>
    public string Destino { get; set; }
    /// <summary>
    /// Descripcion del motivo del viaje
    /// </summary>
    public string Motivo { get; set; }
    /// <summary>
    /// Indica el estado del vehiculo
    /// </summary>
    /// <remarks>
    /// 0 = Pendiente, 1 = Aprobada, 2 = Rechazada, 3 = Finalizada
    /// </remarks>
    public int Estado { get; set; }
    /// <summary>
    /// Identificador unico del usuario solicitante
    /// </summary>
    public int IdSolicitante { get; set; }
    /// <summary>
    /// Identificador unico del vehículo
    /// </summary>
    public int IdVehiculo { get; set; }
    /// <summary>
    /// Identificador unico del conductor
    /// </summary>
    public int? IdConductor { get; set; }
    public string Trial826 { get; set; }
    /// <summary>
    /// Relacion uno es a mucho de la aprobación
    /// </summary>
    public virtual ICollection<Aprobacione> Aprobaciones { get; set; } = new List<Aprobacione>();
    /// <summary>
    /// Relacion uno es a mucho de la bitácora
    /// </summary>
    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();
    /// <summary>
    /// Relacion uno es a uno del conductor
    /// </summary>
    public virtual Conductore IdConductorNavigation { get; set; }
    /// <summary>
    /// Relacion uno es a uno del usuario solicitante
    /// </summary>
    public virtual Usuario IdSolicitanteNavigation { get; set; }
    /// <summary>
    /// Relacion uno es a uno del vehículo
    /// </summary>
    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
