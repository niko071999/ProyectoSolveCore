namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa la tabla intermedia entre Usuario y Rol. 
/// Contiene los identificadores únicos de ambos
/// </summary>
public partial class Usuariosrole
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Identificador único del usuario
    /// </summary>
    public int Idusuario { get; set; }
    /// <summary>
    /// Identificador único del rol
    /// </summary>
    public int Idrol { get; set; }
    /// <summary>
    /// Relación uno es a uno del rol
    /// </summary>
    public virtual Role IdrolNavigation { get; set; }
    /// <summary>
    /// Relación uno es a uno del usuario
    /// </summary>
    public virtual Usuario IdusuarioNavigation { get; set; }
}
