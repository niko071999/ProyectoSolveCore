namespace ProyectoSolveCore.Models.ViewModels
{
    /// <summary>
    /// Clase que recibe el identificado único de la solicitud y el motivo del rechazo de la solicitud
    /// </summary>
    public class vmRechazarSolicitud
    {
        public int id_solicitud { get; set; }
        public string motivo { get; set; }
    }
}