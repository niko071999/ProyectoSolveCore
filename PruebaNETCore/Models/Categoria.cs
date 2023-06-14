using System;
using System.Collections.Generic;

namespace ProyectoSolveCore.Models;
/// <summary>
/// Clase que representa las categorias de los véhículos
/// </summary>
public partial class Categoria
{
    /// <summary>
    /// Identificador unico
    /// </summary>
    public int Id { get; set; } 
    /// <summary>
    /// Nombre de la categoría
    /// </summary>
    public string Categoria1 { get; set; }
    public string Trial820 { get; set; }
    /// <summary>
    /// Relacion uno es a uno con el vehículo
    /// </summary>
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
