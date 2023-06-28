namespace ProyectoSolveCore.Models;
public partial class Usuario
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Numero de identificación del usuario
    /// </summary>
    public string Rut { get; set; }
    /// <summary>
    /// Nombre de la persona/usuario
    /// </summary>
    public string Nombre { get; set; }
    /// <summary>
    /// Apellido de la persona/usuario
    /// </summary>
    public string Apellido { get; set; }
    /// <summary>
    /// Clave de acceso del usuario
    /// </summary>
    public string Clave { get; set; }
    /// <summary>
    /// Indica si el usuario está habilitado para ingresar al sistema
    /// </summary>
    public bool Login { get; set; }
    /// <summary>
    /// Indica si el usuario esta eliminado
    /// </summary>
    public bool Eliminado { get; set; }
    /// <summary>
    /// Identificador único del departamento
    /// </summary>
    public int IdDepartamento { get; set; }
    /// <summary>
    /// Dirección de la imagen
    /// </summary>
    public string DireccionImg { get; set; }
    /// <summary>
    /// Relación uno es a mucho de la aprobación
    /// </summary>
    public virtual ICollection<Aprobacion> Aprobaciones { get; set; } = new List<Aprobacion>();
    /// <summary>
    /// Relación uno es a mucho del conductor
    /// </summary>
    public virtual ICollection<Conductor> Conductores { get; set; } = new List<Conductor>();
    /// <summary>
    /// Relación uno es a uno del departamento
    /// </summary>
    public virtual Departamento IdDepartamentoNavigation { get; set; }
    /// <summary>
    /// Relación uno es a mucho de la solicitud
    /// </summary>
    public virtual ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
    /// <summary>
    /// Relación uno es a mucho de la clase "Usuariosrole"
    /// </summary>
    public virtual ICollection<Usuariosrole> Usuariosroles { get; set; } = new List<Usuariosrole>();
}
