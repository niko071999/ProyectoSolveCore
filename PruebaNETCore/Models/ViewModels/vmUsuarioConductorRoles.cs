using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que recibe los datos de los usuarios al añadir o al editarlos
    /// </summary>
    public class vmUsuarioConductorRoles
    {
        public int ID { get; set; }
        /// <summary>
        /// Permite verificar si el RUT del usuario a sido cambiado 
        /// para verificar si existe en la base de datos
        /// </summary>
        public string rutold { get; set; }
        public string rut { get; set; }
        public string clave { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public bool login { get; set; }
        public bool eliminado { get; set; }
        public string direccion_img { get; set; }
        public int id_departamento { get; set; }
        public int? id_conductor { get; set; }
        public int? id_vehiculo { get; set; }
        public int? NumeroPoliza { get; set; }
        public DateTime? FechaEmitida { get; set; }
        public DateTime? FecheVencimiento { get; set; }
        /// <summary>
        /// Indica si tiene rol de administrador
        /// </summary>
        public bool RolAdministrador { get; set; }
        /// <summary>
        /// Indica si tiene rol de jefe
        /// </summary>
        public bool RolJefe { get; set; }
        /// <summary>
        /// Indica si tiene rol de mantenedor de usuarios
        /// </summary>
        public bool RolMantendorUsuarios { get; set; }
        /// <summary>
        /// Indica si tiene rol de mantenedor de vehículos no pesados
        /// </summary>
        public bool RolMantenedorVehiculos { get; set; }
        /// <summary>
        /// Indica si tiene rol de solicitador
        /// </summary>
        public bool RolSolicitador { get; set; }
        /// <summary>
        /// Indica si tiene rol de vehículos pesados
        /// </summary>
        public bool RolMantenedorVehiculosMaq { get; set; }
        /// <summary>
        /// Indica si tiene rol de la bitácora
        /// </summary>
        public bool RolMantenedorBitacora { get; set; }
    }
}