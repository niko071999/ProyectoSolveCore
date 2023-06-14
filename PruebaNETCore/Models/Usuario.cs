namespace ProyectoSolveCore.Models;
public partial class Usuario
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Numero de identificacion del usuario
    /// </summary>
    public string Rut { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Clave { get; set; }
    /// <summary>
    /// Indica si el usuario esta habilitado para ingresar al sistema
    /// </summary>
    public bool Login { get; set; }
    /// <summary>
    /// Indica si el usuario esta eliminado
    /// </summary>
    public bool Eliminado { get; set; }
    /// <summary>
    /// Identificador unico del departamento
    /// </summary>
    public int IdDepartamento { get; set; }
    /// <summary>
    /// Direccion de la imagen
    /// </summary>
    public string DireccionImg { get; set; }
    public string Trial826 { get; set; }
    /// <summary>
    /// Relacion uno es a mucho de la aprobación
    /// </summary>
    public virtual ICollection<Aprobacione> Aprobaciones { get; set; } = new List<Aprobacione>();
    /// <summary>
    /// Relacion uno es a mucho del conductor
    /// </summary>
    public virtual ICollection<Conductore> Conductores { get; set; } = new List<Conductore>();
    /// <summary>
    /// Relacion uno es a uno del departamento
    /// </summary>
    public virtual Departamento IdDepartamentoNavigation { get; set; }
    /// <summary>
    /// Relacion uno es a mucho de la solicitud
    /// </summary>
    public virtual ICollection<Solicitude> Solicitudes { get; set; } = new List<Solicitude>();
    /// <summary>
    /// Relacion uno es a mucho de la clase "Usuariosrole"
    /// </summary>
    public virtual ICollection<Usuariosrole> Usuariosroles { get; set; } = new List<Usuariosrole>();
}
