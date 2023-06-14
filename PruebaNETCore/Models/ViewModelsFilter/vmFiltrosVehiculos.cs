namespace ProyectoSolveCore.Models.ViewModelsFilter
{
    /// <summary>
    /// Representa los datos visualizados para filtras los vehículo
    /// </summary>
    public class vmFiltrosVehiculos
    {
        public string Patente { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        /// <summary>
        /// Opcion seleccionada del estado del vehículo
        /// </summary>
        /// <remarks>
        /// 1 = Habilitado, 2 = Precaucion, 3 = Deshabilitado
        /// </remarks>
        public int OpcionEstado { get; set; }
        /// <summary>
        /// Identificador unico de la categoria
        /// </summary>
        public int IdCategoria { get; set; }
    }
}
