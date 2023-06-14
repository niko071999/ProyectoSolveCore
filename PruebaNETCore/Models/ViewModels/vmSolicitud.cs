namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que muestra los datos de la solicitud en una tabla
    /// </summary>
    [Serializable]
    public class vmSolicitud
    {
        public int id { get; set; }
        public DateTime FechaSolicitado { get; set; }
        public DateTime FechaSalida { get; set; }
        public string fechaLongSalidaStr { get; set; }
        public string fechaSalidaStr { get; set; }
        public DateTime FechaLlegada { get; set; }
        public string fechaLongLlegadaStr { get; set; }
        public string fechaLlegadaStr { get; set; }
        public string nombreConductor { get; set; }
        public string vehiculo { get; set; }
        public string motivo { get; set; }
        public string NombreSolicitante { get; set; }
        public int Estado { get; set; }
        public int CantidadAprobacion { get; set; }

        public vmSolicitud()
        {

        }

        public vmSolicitud(int Id, string FechaLongSalidaStr, string FechaSalidaStr, string FechaLongLlegadaStr,
            string FechaLlegadaStr, string NombreConductor, string Vehiculo, string Motivo)
        {
            id = Id;
            fechaLongSalidaStr = FechaLongSalidaStr;
            fechaSalidaStr = FechaSalidaStr;
            fechaLongLlegadaStr = FechaLongLlegadaStr;
            fechaLlegadaStr = FechaLlegadaStr;
            nombreConductor = NombreConductor;
            vehiculo = Vehiculo;
            motivo = Motivo;
        }

        public vmSolicitud(int Id, DateTime FechaSolicitado, DateTime FechaSalida, DateTime FechaLlegada,
            string Vehiculo, string NombreConductor, int Estado, int numeroAprobacion)
        {
            id = Id;
            this.FechaSolicitado = FechaSolicitado;
            this.FechaSalida = FechaSalida;
            this.FechaLlegada = FechaLlegada;
            vehiculo = Vehiculo;
            nombreConductor = string.IsNullOrEmpty(NombreConductor) ?
                "Sin conductor por asignar" : NombreConductor;
            this.Estado = Estado;
            CantidadAprobacion = numeroAprobacion;
        }
        public vmSolicitud(int Id, DateTime FechaSolicitado, DateTime FechaSalida, DateTime FechaLlegada, string Vehiculo,
            string NombreSolicitante, string NombreConductor, int Estado)
        {
            id = Id;
            this.FechaSolicitado = FechaSolicitado;
            this.FechaSalida = FechaSalida;
            this.FechaLlegada = FechaLlegada;
            vehiculo = Vehiculo;
            this.NombreSolicitante = NombreSolicitante;
            nombreConductor = string.IsNullOrEmpty(NombreConductor) ?
                "Sin conductor por asignar" : NombreConductor;
            this.Estado = Estado;
        }
    }
}