using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.NewModels;

/// <summary>
/// TRIAL
/// </summary>
public partial class Usuario
{
    public int Id { get; set; }

    public string Rut { get; set; }

    public string Nombre { get; set; }

    public string Apellido { get; set; }

    public string Clave { get; set; }

    public bool Login { get; set; }

    public bool Eliminado { get; set; }

    public int IdDepartamento { get; set; }

    public string DireccionImg { get; set; }

    public string Trial826 { get; set; }

    public virtual ICollection<Aprobacione> Aprobaciones { get; set; } = new List<Aprobacione>();

    public virtual ICollection<Conductore> Conductores { get; set; } = new List<Conductore>();

    public virtual Departamento IdDepartamentoNavigation { get; set; }

    public virtual ICollection<Solicitude> Solicitudes { get; set; } = new List<Solicitude>();

    public virtual ICollection<Usuariosrole> Usuariosroles { get; set; } = new List<Usuariosrole>();
}
