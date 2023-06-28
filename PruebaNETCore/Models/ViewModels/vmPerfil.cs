namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase recibe los datos del perfil del usuario en sesión
    /// </summary>
    public class vmPerfil
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string RutOld { get; set; }
        public string Rut { get; set; }
        public string Clave { get; set; }
        public string Apellido { get; set; }
        public bool Eliminado { get; set; }
        public string DireccionImg { get; set; }
        public int IdDepartamento { get; set; }
        public int? IdConductor { get; set; }
        public int? IdVehiculo { get; set; }
        public int? NumeroPoliza { get; set; }
        public DateTime? FechaEmitida { get; set; }
        public DateTime? FecheVencimiento { get; set; }
        /// <summary>
        /// Indica si tiene rol de administrador
        /// </summary>
        public bool RolAdministrador { get; set; }
        /// <summary>
        /// Indica si tiene rol jefe
        /// </summary>
        public bool RolJefe { get; set; }
        /// <summary>
        /// Indica si tiene rol de mantenedor de usuarios
        /// </summary>
        public bool RolMantendorUsuarios { get; set; }
        /// <summary>
        /// Indica si tiene rol de mantenedor de vehículo no maquinaras
        /// </summary>
        public bool RolMantenedorVehiculos { get; set; }
        /// <summary>
        /// Indica si tiene rol solicitador
        /// </summary>
        public bool RolSolicitador { get; set; }
        /// <summary>
        /// Indica si tiene rol mantenedor de vehículos maquinarias
        /// </summary>
        public bool RolMantenedorVehiculosMaq { get; set; }
        /// <summary>
        /// Indica si tiene rol de mantenedor de la bitácora
        /// </summary>
        public bool RolMantenedorBitacora { get; set; }
    }
}
