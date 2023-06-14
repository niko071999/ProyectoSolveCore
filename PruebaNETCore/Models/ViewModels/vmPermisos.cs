using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que muestra los permisos del usuario con un icono
    /// </summary>
    public class VmPermiso
    {
        public int Id { get; set; }
        public string Permiso { get; set; }
        /// <summary>
        /// Cadena de texto la cual etiquetas de iconos HTML
        /// </summary>
        public string IconPermiso { get; set; }
    }
}