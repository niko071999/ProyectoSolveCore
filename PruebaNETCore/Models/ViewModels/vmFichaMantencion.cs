namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase para mostrar las mantenciones en una tabla
    /// </summary>
    public class vmFichaMantencion
    {
        public int Id { get; set; }
        public DateTime FechaMantencion { get; set; }
        public int IdVehiculo { get; set; }
        public string Vehiculo { get; set; }
        public int Kilometraje { get; set; }
        public int IdConductor { get; set; }
        public string NombreConductor { get; set; }
        public string Descripcion { get; set; }
    }
}
