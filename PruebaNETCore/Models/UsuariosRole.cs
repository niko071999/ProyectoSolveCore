namespace ProyectoSolveCore.Models;
/// <summary>
/// Representa la tabla intermedia entre Usuario y Rol. 
/// Contiene los identificadores unicos de ambos
/// </summary>
public partial class Usuariosrole
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Identificador unico del usuario
    /// </summary>
    public int Idusuario { get; set; }
    /// <summary>
    /// Identificador unico del rol
    /// </summary>
    public int Idrol { get; set; }
    public string Trial829 { get; set; }
    /// <summary>
    /// Relacion uno es a uno del rol
    /// </summary>
    public virtual Role IdrolNavigation { get; set; }
    /// <summary>
    /// Relacion uno es a uno del usuario
    /// </summary>
    public virtual Usuario IdusuarioNavigation { get; set; }
}
