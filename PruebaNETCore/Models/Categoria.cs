using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
/// <summary>
/// Clase que representa las categorías de los vehículos
/// </summary>
public partial class Categoria
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; } 
    /// <summary>
    /// Nombre de la categoría
    /// </summary>
    public string Categoria1 { get; set; }
    /// <summary>
    /// Relación uno es a uno con el vehículo
    /// </summary>
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
