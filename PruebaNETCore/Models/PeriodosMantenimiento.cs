namespace ProyectoSolveCore.Models;
public partial class Periodosmantenimiento
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }
    public int PeriodoKilometraje { get; set; }
    public string Trial823 { get; set; }
    /// <summary>
    /// Relacion uno es a mucho del vehículo
    /// </summary>
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
