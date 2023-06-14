using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.NewModels;

/// <summary>
/// TRIAL
/// </summary>
public partial class Aprobacione
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public bool Estado { get; set; }

    public string Motivo { get; set; }

    public int IdJefe { get; set; }

    public int IdSolicitud { get; set; }

    public string Trial816 { get; set; }

    public virtual Usuario IdJefeNavigation { get; set; }

    public virtual Solicitude IdSolicitudNavigation { get; set; }
}
