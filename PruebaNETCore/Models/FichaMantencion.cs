namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa la entidad de mantención realizada al vehículo
/// </summary>
public partial class Fichamantencion
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Fecha de la mantención
    /// </summary>
    public DateTime FechaMantencion { get; set; }
    /// <summary>
    /// Indica el kilometraje del vehículo al hacer la mantención
    /// </summary>
    public int Kilometraje { get; set; }
    /// <summary>
    /// Descripción de la mantención realizada al vehículo
    /// </summary>
    public string Descripcion { get; set; }
    /// <summary>
    /// Identificador único del conductor
    /// </summary>
    public int IdConductor { get; set; }
    /// <summary>
    /// Identificador único del vehículo
    /// </summary>
    public int IdVehiculo { get; set; }
    public string Trial823 { get; set; }
    /// Relación uno es a uno con el conductor
    public virtual Conductor IdConductorNavigation { get; set; }
    /// Relación uno es a uno con el vehículo
    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
