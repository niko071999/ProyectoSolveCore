namespace ProyectoSolveCore.Models.ViewModels
{
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
        public bool RolAdministrador { get; set; }
        public bool RolJefe { get; set; }
        public bool RolMantendorUsuarios { get; set; }
        public bool RolMantenedorVehiculos { get; set; }
        public bool RolSolicitador { get; set; }
        public bool RolMantenedorVehiculosMaq { get; set; }
        public bool RolMantenedorBitacora { get; set; }
    }
}
