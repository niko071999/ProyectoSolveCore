namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase para recibir la solicitud y un atributo boleano
    /// </summary>
    /// <remarks>
    /// Permite mostrar más informacion de la solicitud
    /// </remarks>
    public class vmMasInformacionSolicitud
    {
        /// <summary>
        /// Objeto solicitud
        /// </summary>
        public Solicitude solicitud { get; set; }
        /// <summary>
        /// Indica si el usuario esta aprobando la solicitud
        /// </summary>
        public bool aprobacion { get; set; }
    }
}