namespace ProyectoSolveCore.Models;
/// <summary>
/// Reprenda los departamentos/área de los usuarios
/// </summary>
public partial class Departamento
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Nombre del departamento o área
    /// </summary>
    public string Departamento1 { get; set; }
    /// <summary>
    /// Relación uno es a muchos con el usuario
    /// </summary>
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
