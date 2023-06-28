namespace ProyectoSolveCore.Models;
public partial class Bitacora
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// Folio de la bitácora
    /// </summary>
    public long Folio { get; set; }
    /// <summary>
    /// Fecha de creación de la bitácora
    /// </summary>
    public DateTime Fecha { get; set; }
    /// <summary>
    /// Litros ingresados al vehículo
    /// </summary>
    public int? LitrosCombustible { get; set; }
    public string Observacion { get; set; }
    /// <summary>
    /// Identificador único del vehículo
    /// </summary>
    public int IdVehiculo { get; set; }
    /// <summary>
    /// Identificador único del conductor
    /// </summary>
    public int IdConductor { get; set; }
    /// <summary>
    /// Identificado único de la solicitud
    /// </summary>
    public int IdSolicitud { get; set; }
    /// <summary>
    /// Identificador único del kilometraje
    /// </summary>
    public int IdKilometraje { get; set; }

    /// <summary>
    /// Relación uno es a uno con el conductor
    /// </summary>
    public virtual Conductor IdConductorNavigation { get; set; }
    /// <summary>
    /// Relación uno es a uno con el kilometraje
    /// </summary>
    public virtual Kilometraje IdKilometrajeNavigation { get; set; }
    /// <summary>
    /// Relación uno es a uno con la solicitud
    /// </summary>
    public virtual Solicitud IdSolicitudNavigation { get; set; }
    /// <summary>
    /// Relación uno es a uno con el vehículo
    /// </summary>
    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
