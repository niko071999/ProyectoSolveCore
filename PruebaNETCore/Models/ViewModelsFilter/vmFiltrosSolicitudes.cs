﻿namespace ProyectoSolveCore.Models.ViewModelsFilter
{
    /// <summary>
    /// Representa los datos visualizados para filtras las solicitudes
    /// </summary>
    public class vmFiltrosSolicitudes
    {
        public int Estado { get; set; }
        public string Motivo { get; set; }
        /// <summary>
        /// Represta la opcion selecciona para filtrar la fecha
        /// </summary>
        public int OpcionFecha { get; set; }
        public string Destino { get; set; }
        public string Vehiculo { get; set; }
        /// <summary>
        /// Identificador unico del usuario solicitante
        /// </summary>
        public int IdSolicitado { get; set; }
        //public DateTime FechaDesde { get; set; }
        //public DateTime FechaHasta { get; set; }
    }
}
