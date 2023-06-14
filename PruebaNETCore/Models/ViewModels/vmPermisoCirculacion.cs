namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que muestra los datos del Permiso de Circulación en una pagina
    /// </summary>
    public class vmPermisoCirculacion
    {
        public string vehiculo { get; set; }
        public string motivo { get; set; }
        public string nombreConductor { get; set; }
        /// <summary>
        /// Nombre de los pasajeros
        /// </summary>
        public string pasajeros { get; set; }
        public string destino { get; set; }
        /// <summary>
        /// Representa la municipalidad
        /// </summary>
        public string servicio { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegada { get; set; }
        /// <summary>
        /// Representa la nota importante del documento
        /// </summary>
        public string Notas { get; set; }
        public string NombreFirma { get; set; }

        public vmPermisoCirculacion()
        {
            servicio = "I. MUNICIPALIDAD DE PELARCO";
        }
    }
}
