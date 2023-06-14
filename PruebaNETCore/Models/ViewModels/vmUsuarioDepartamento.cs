namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que permite mostrar el usuario con su departamento
    /// </summary>
    public class vmUsuarioDepartamento
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public int IdDepartamento { get; set; }
    }
}
