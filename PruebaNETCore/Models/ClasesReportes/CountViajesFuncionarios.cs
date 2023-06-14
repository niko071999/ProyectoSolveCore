namespace ProyectoSolveCore.Models.ClasesReportes
{
    /// <summary>
    /// Representa la cantidad de viajes recorridos por los funcionarios
    /// </summary>
    public class CountViajesFuncionarios
    {
        /// <summary>
        /// Departamento a que pertenece el usuario
        /// </summary>
        public string Departamento { get; set; }
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Cantidad de viajes realizados por el usuario
        /// </summary>
        public int NumeroViaje { get; set; }
    }
}
