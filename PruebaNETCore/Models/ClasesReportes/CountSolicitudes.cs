namespace ProyectoSolveCore.Models.ClasesReportes
{
    /// <summary>
    /// Representa las cantidades de las solicitudes en sus estados
    /// </summary>
    public class CountSolicitudes
    {
        /// <summary>
        /// Total de solicitudes creadas
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// Total de solicitudes creadas pendientes
        /// </summary>
        public int TotalPendientes { get; set; }
        /// <summary>
        /// Total de solicitudes creadas aprobadas
        /// </summary>
        public int TotalAprobadas { get; set; }
        /// <summary>
        /// Total de solicitudes creadas rechazadas
        /// </summary>
        public int TotalRechazadas { get; set; }
        /// <summary>
        /// Total de solicitudes creadas finalizadas
        /// </summary>
        public int TotalFinalizadas { get; set; }
    }
}
