namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase para recibir la solicitud y un atributo bolean
    /// </summary>
    /// <remarks>
    /// Permite mostrar más información de la solicitud
    /// </remarks>
    public class vmMasInformacionSolicitud
    {
        /// <summary>
        /// Objeto solicitud
        /// </summary>
        public Solicitud solicitud { get; set; }
        /// <summary>
        /// Indica si el usuario esta aprobando la solicitud
        /// </summary>
        public bool aprobacion { get; set; }
    }
}