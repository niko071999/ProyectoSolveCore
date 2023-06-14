﻿using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
public partial class Kilometraje
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Fecha de cracion del registro del kilometraje
    /// </summary>
    public DateTime FechaCreacion { get; set; }
    public int KilometrajeInicial { get; set; }
    public int KilometrajeFinal { get; set; }
    /// <summary>
    /// Identificador unico del vehículo
    /// </summary>
    public int IdVehiculo { get; set; }
    public string Trial823 { get; set; }

    /// <summary>
    /// Relacion uno es a muchos con la bitácora
    /// </summary>
    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();
    /// Relacion uno es a uno con el vehículo
    public virtual Vehiculo IdVehiculoNavigation { get; set; }
}
