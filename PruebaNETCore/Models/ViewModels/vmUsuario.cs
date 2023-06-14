using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que muestra los datos de los usuarios en una tabla
    /// </summary>
    public class vmUsuario
    {
        public int Id { get; set; }
        public string Rut { get; set; }
        public string Nombre { get; set; }
        public string Departamento { get; set; }
        public Vehiculo NombreVehiculo { get; set; }
        public List<vmRol> Roles { get; set; }
        public List<VmPermiso> Permisos { get; set; }
        public bool Conductor { get; set; }
        public string clave { get; set; }
        public bool? eliminado { get; set; }
        public int? direccion_img { get; set; }
        public int? id_departamento { get; set; }
    }
}