namespace ProyectoSolveCore.Models
{
    /// <summary>
    /// Representa los mensaje en formato JSON enviados a la vista
    /// </summary>
    public class JsonMensaje
    {
        /// <summary>
        /// Contenido del mensaje
        /// </summary>
        public string Mensaje { get; set; }
        /// <summary>
        /// Tipo del mensaje: error o success
        /// </summary>
        public string Type { get; set; }
    }
}
