namespace ProyectoSolveCore.Models
{
    /// <summary>
    /// Representa entidad para crear una lista SelectList con Grupos 
    /// y cargarla en un Select HTML
    /// </summary>
    public class SelectListWithGroups
    {
        /// <summary>
        /// Valor del item
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Texto que se muestra en la lista
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Grupo
        /// </summary>
        public string Group { get; set; }
    }
}
