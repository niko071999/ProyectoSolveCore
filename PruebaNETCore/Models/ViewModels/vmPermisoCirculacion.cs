namespace ProyectoSolveCore.Models.ViewModels
{
    public class vmPermisoCirculacion
    {
        public string vehiculo { get; set; }
        public string motivo { get; set; }
        public string nombreConductor { get; set; }
        public string pasajeros { get; set; }
        public string destino { get; set; }
        public string servicio { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegada { get; set; }
        public string Notas { get; set; }
        public string NombreFirma { get; set; }

        public vmPermisoCirculacion()
        {
            servicio = "I. MUNICIPALIDAD DE PELARCO";
        }
    }
}
