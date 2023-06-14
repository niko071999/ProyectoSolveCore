namespace ProyectoSolveCore.Models;
/// <summary>
/// Reprenta los departamentos/area de los usuarios
/// </summary>
public partial class Departamento
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Nombre del departamento o area
    /// </summary>
    public string Departamento1 { get; set; }
    public string Trial820 { get; set; }
    /// <summary>
    /// Relacion uno es a muchos con el usuario
    /// </summary>
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
