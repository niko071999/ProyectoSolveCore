﻿namespace ProyectoSolveCore.Models.ViewModels
{
    public class vmBitacora
    {
        public int Id { get; set; }
        public int IdConductor { get; set; }
        public int IdSolicitud { get; set; }
        public int IdVehiculo { get; set; }
        public int Folio { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegada { get; set; }
        public string NombreCompletoConductor { get; set; }
        public string Vehiculo { get; set; }
        public string Destino { get; set; }
        public string Motivo { get; set; }
        public int KmInicialEntero { get; set; }
        public int KmInicialDecimal { get; set; }
        public int KmFinalEntero { get; set; }
        public int KmFinalDecimal { get; set; }
        public string Combustible { get; set; }
        public string Observacion { get; set; }
    }
}
