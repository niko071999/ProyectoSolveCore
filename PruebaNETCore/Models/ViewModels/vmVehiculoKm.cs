using System.ComponentModel;

namespace ProyectoSolveCore.Models.ViewModels
{
    public class vmVehiculoKm
    {
        public int Id { get; set; }

        public string Patente { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }
        public bool Estado { get; set; } = true;

        public bool Eliminado { get; set; }

        public int IdPeriodoKilometraje { get; set; }

        public string? DireccionImg { get; set; }
        public int KilometrajeInicial { get; set; }
        public int KilometrajeInicialDecimal { get; set; }

    }

}