namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase para recibir los indentificadores unicos del conductor y 
    /// de la solicitud cuando se aprueban o rechazan las solicitudes
    /// </summary>
    public class vmIdConductorSolicitud
    {
        public int? IdConductor { get; set; }
        public int IdSolicitud { get; set; }
    }
}
