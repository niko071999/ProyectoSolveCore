namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que se ocupa para mostrar el reporte de cantidad de solicitudes
    /// </summary>
    public class vmCantidadSolicitudes
    {
        public DateTime Mes { get; set; }
        public int TotalSolicitudes { get; set; }
        public int TotalSolicitudesPend { get; set; }
        public int TotalSolicitudesAprob { get; set; }
        public int TotalSolicitudesRech { get; set; }
        public int TotalSolicitudesFin { get; set; }
        public decimal PorcentajePendiente { get; set; }
        public decimal PorcentajeAprobado { get; set; }
        public decimal PorcentajeRechazado { get; set; }
        public decimal PorcentajeFinalizado { get; set; }

    }
}
