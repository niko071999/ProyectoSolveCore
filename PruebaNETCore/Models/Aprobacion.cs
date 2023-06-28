namespace ProyectoSolveCore.Models;
public partial class Aprobacion
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Fecha de creación de la aprobación
    /// </summary>
    public DateTime Fecha { get; set; }
    /// <summary>
    /// Resultado de la aprobación, True: Aprobada - False: Rechazada
    /// </summary>
    public bool Estado { get; set; }
    /// <summary>
    /// Motivo del rechazo de la solicitud
    /// </summary>
    public string Motivo { get; set; }
    /// <summary>
    /// Identificador único del usuario quien aprobó
    /// </summary>
    public int IdJefe { get; set; }
    /// <summary>
    /// Identificador único de la solicitud
    /// </summary>
    public int IdSolicitud { get; set; }
    /// <summary>
    /// Relación uno es a uno con el usuario
    /// </summary>
    public virtual Usuario IdJefeNavigation { get; set; }
    /// <summary>
    /// Relación uno es a uno con la solicitud
    /// </summary>
    public virtual Solicitud IdSolicitudNavigation { get; set; }
}
