namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que permite verificar el rol del usuario
    /// </summary>
    public class vmRolCheck
    {
        public int IdRol { get; set; }
        public int IdUsuario { get; set; }
        public bool check { get; set; }
    }
}
