namespace ProyectoSolveCore.Models.ViewModels
{
    public class vmFechaMantencion
    {
        public int Id { get; set; }

        public DateTime FechaMantencion { get; set; }

        public int Kilometraje { get; set; }

        public string Descripcion { get; set; }
        
        public string NombreConductor { get; set; }

        public int IdConductor { get; set; }

        public string Vehiculo { get; set; }
        
        public int IdVehiculo { get; set; }

    }
}
