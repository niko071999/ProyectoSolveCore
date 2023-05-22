namespace ProyectoSolveCore.Models.ViewModels
{
    public class vmFichaMantencion
    {
        public int Id { get; set; }
        public DateTime FechaMantencion { get; set; }
        public int IdVehiculo { get; set; }
        public string Vehiculo { get; set; }
        public decimal Kilometraje { get; set; }
        public int IdConductor { get; set; }
        public string NombreConductor { get; set; }
        public string Descripcion { get; set; }
    }
}
