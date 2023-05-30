using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSolveCore.Models.ViewModels
{
    public class vmUsuarioConductorRoles
    {
        public int ID { get; set; }
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
        public bool RolAdministrador { get; set; }
        public bool RolJefe { get; set; }
        public bool RolMantendorUsuarios { get; set; }
        public bool RolMantenedorVehiculos { get; set; }
        public bool RolSolicitador { get; set; }
        public bool RolMantenedorVehiculosMaq { get; set; }
        public bool RolMantenedorBitacora { get; set; }
    }
}