namespace ProyectoSolveCore.Models.ViewModels
{
    public class vmBitacoraInfo
    {
        public long Id { get; set; }
        public DateTime FechaBitacora { get; set; }
        public DateTime FechaSolicitado { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegada { get; set; }
        public int LitrosCombustible { get; set; }
        public string Observacion { get; set; }
        public string Solicitante { get; set; }
        public string Conductor { get; set; }
        public string Vehiculo { get; set; }
        public int KmInicial { get; set; }
        public int KmFinal { get; set; }
    }
}
