namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que muestra los roles del usuario con un icono
    /// </summary>
    public class vmRol
    {
        public int Id { get; set; }
        public string rol { get; set; }
        /// <summary>
        /// Cadena de texto la cual etiquetas de iconos HTML
        /// </summary>
        public string IconRol { get; set; }
    }
}
