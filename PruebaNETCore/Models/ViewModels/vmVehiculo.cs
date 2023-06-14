using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que muestra los datos de los vehículos en una tabla
    /// </summary>
    public class vmVehiculo
    {
        public int Id { get; set; }
        public string Patente { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Year { get; set; }
        public string NombreConductor { get; set; }
        public int IdCategoria { get; set; }
        public int Estado { get; set; }
        public string MensajeEstado { get; set; }
        public int? Km_Recorrido { get; set; }
    }
}