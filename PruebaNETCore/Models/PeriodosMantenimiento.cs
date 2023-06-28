namespace ProyectoSolveCore.Models;
public partial class Periodosmantenimiento
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    public int PeriodoKilometraje { get; set; }
    /// <summary>
    /// Relación uno es a mucho del vehículo
    /// </summary>
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
