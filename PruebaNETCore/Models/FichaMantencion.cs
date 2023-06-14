namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa las mantenciones realizadas a los vehículos
/// </summary>
public partial class Fichamantencion
{
    /// <summary>
    /// Identificador unico
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
    /// Descripcion de la mantención realizada al vehículo
    /// </summary>
    public string Descripcion { get; set; }
    /// <summary>
    /// Identificador unico del conductor
    /// </summary>
    public int IdConductor { get; set; }
    /// <summary>
    /// Identificador unico del vehículo
    /// </summary>
    public int IdVehiculo { get; set; }
    public string Trial823 { get; set; }
    /// Relacion uno es a uno con el conductor
    public virtual Conductore IdConductorNavigation { get; set; }
    /// Relacion uno es a uno con el vehículo
    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
