namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que sirve para recibir dos fechas cuando 
    /// se verifican los vehiculos disponibles
    /// </summary>
    [Serializable]
    public class vmFechaSalidaLlegada
    {
        public string fecha_llegada { get; set; }
        public string fecha_salida { get; set; }
    }
}