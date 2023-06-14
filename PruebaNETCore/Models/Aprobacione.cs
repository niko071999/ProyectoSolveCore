using ProyectoSolveCore.Models;
using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
public partial class Aprobacione
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Fecha de cracion de la aprobación
    /// </summary>
    public DateTime Fecha { get; set; }
    /// <summary>
    /// Resultado de la aprobacion, True: Aprobada - False: Rechazada
    /// </summary>
    public bool Estado { get; set; }
    /// <summary>
    /// Motivo del rechazo de la solicitud
    /// </summary>
    public string Motivo { get; set; }
    /// <summary>
    /// Identificador unico del usuario quien aprobó
    /// </summary>
    public int IdJefe { get; set; }
    /// <summary>
    /// Identificador unico de la solicitud
    /// </summary>
    public int IdSolicitud { get; set; }
    public string Trial816 { get; set; }
    /// <summary>
    /// Relacion uno es a uno con el usuario
    /// </summary>
    public virtual Usuario IdJefeNavigation { get; set; }
    /// <summary>
    /// Relacion uno es a uno con la solicitud
    /// </summary>
    public virtual Solicitude IdSolicitudNavigation { get; set; }
}
