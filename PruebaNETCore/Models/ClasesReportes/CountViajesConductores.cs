namespace ProyectoSolveCore.Models.ClasesReportes
{
    /// <summary>
    /// Representa la cantidad de viajes y de kilómetros recorridos por los conductores
    /// </summary>
    public class CountViajesConductores : CountViajesFuncionarios
    {
        /// <summary>
        /// Kilometraje total recorrido por le conductor
        /// </summary>
        public int KilometrajesTotales { get; set; }
    }
}
