using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con los kilometrajes.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver, crear, modificar y eliminar 
    /// registros de kilometrajes de los vehículos.
    /// </remarks>
    [Authorize]
    public class KilometrajeController : Controller
    {
        /// <summary>
        /// contexto de la base de datos
        /// </summary>
        private readonly ModelData _context;
        //Constructor
        public KilometrajeController(ModelData context)
        {
            _context = context;
        }
        /// <summary>
        /// Método que muestra la opción de ver los kilómetros de un vehículo
        /// </summary>
        /// <returns>Vista con la opción de mostrar los kilómetros de un vehículo seleccionado</returns>
        [Autorizar(permisoId: 20)]
        public async Task<IActionResult> VisualizarHistorialKm()
        {
            var vlist = await ObtenerVehiculos();
            ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", null, "Group");
            return View(new List<Kilometraje>());
        }
        /// <summary>
        /// Método que muestra los kilometrajes de un vehículo.
        /// </summary>
        /// <param name="km">Kilometraje del vehículo</param>
        /// <returns>Una vista con una lista de kilometrajes del vehículo</returns>
        [Autorizar(permisoId: 20)]
        [HttpPost]
        public async Task<IActionResult> VisualizarHistorialKm(Kilometraje km)
        {
            var kilometros = await _context.Kilometrajes.Where(k => k.IdVehiculo == km.IdVehiculo).Include(k => k.IdVehiculoNavigation)
                .ToListAsync();
            var vlist = await ObtenerVehiculos();
            ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", km.IdVehiculo, "Group");
            return View(kilometros);
        }
        /// <summary>
        /// Método que muestra una vista con un formulario para ingresar una entrada de kilometrajes al vehículo
        /// </summary>
        /// <param name="id">Identificador único del vehículo de los kilometrajes</param>
        /// <returns>Una vista con un formulario para ingresar una entrada de kilometrajes</returns>
        [Autorizar(permisoId: 21)]
        public async Task<IActionResult> AgregarKilometrajeVehiculo(int id = -1)
        {
            if (id == -1) //Si es -1 es nulo
            {
                return RedirectToAction("VisualizarHistorialKm");
            }
            var vehiculos = await ObtenerVehiculos();
            if (id == 0)
            {
                ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", null, "Group");
                return View(new Kilometraje());
            }
            ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", id, "Group");
            return View(new Kilometraje());
        }
        /// <summary>
        /// Método que ingresa una entrada de kilometrajes del vehículo.
        /// </summary>
        /// <param name="k">Kilometraje del vehículo</param>
        /// <returns>Una vista, según corresponda el resultado de la operación</returns>
        [Autorizar(permisoId: 21)]
        [HttpPost]
        public async Task<IActionResult> AgregarKilometrajeVehiculo(Kilometraje k)
        {
            try
            {
                var vehiculos = await ObtenerVehiculos();
                if (k == null)
                {
                    ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", null, "Group");
                    return View(new Kilometraje());
                }
                if (k.KilometrajeInicial > k.KilometrajeFinal || k.KilometrajeInicial < 0 || k.KilometrajeFinal < 0)
                {
                    ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", k.IdVehiculo, "Group");
                    return View(k);
                }
                k.FechaCreacion = DateTime.Now;
                await _context.Kilometrajes.AddAsync(k);
                int n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    ViewBag.IdVehiculo = new SelectList(vehiculos, "Value", "Text", k.IdVehiculo, "Group");
                    return View(k);
                }
                bool check = await VerificarMantenimientoVehiculo(k.IdVehiculo);
                if (!check)
                {//Si el vehículo esta deshabilitado, se busca, para cambiar estado
                 //y se guarda los cambios en la base de datos
                    var vehiculo = await _context.Vehiculos.FindAsync(k.IdVehiculo);
                    vehiculo.Estado = false;
                    _context.Vehiculos.Update(vehiculo);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("VisualizarHistorialKm");
            }
            catch (Exception)
            {
                return RedirectToAction("VisualizarHistorialKm");
            }
        }

        /// <summary>
        /// Verifica si un vehículo cumple con el mantenimiento requerido basado en los kilómetros recorridos.
        /// </summary>
        /// <param name="id">El ID del vehículo a verificar.</param>
        /// <returns>Devuelve un valor booleano que indica si el vehículo cumple con el mantenimiento requerido.</returns>
        public async Task<bool> VerificarMantenimientoVehiculo(int id = 0)
        {
            int kmRecorrido = 0;
            //Obtengo las mantenciones del vehículo
            var fichaMant = await _context.Fichamantencions
                .Where(f => f.IdVehiculo == id)
                .ToListAsync();
            //Obtengo el historial de kilometrajes del vehículo
            var kms = await _context.Kilometrajes
                .Where(k => k.IdVehiculo == id)
                .ToListAsync();
            //Obtengo el periodo asignado al vehículo
            var vehiculo = await _context.Vehiculos.Include(v => v.IdPeriodoKilometrajeNavigation).Where(v => v.Id == id).FirstOrDefaultAsync();
            if (vehiculo == null)
            {
                return false;
            }
            //Obtener el periodo del vehículo
            var periodo = vehiculo.IdPeriodoKilometrajeNavigation.PeriodoKilometraje;
            //Verifico si existen mantenciones del vehículo
            if (!fichaMant.Any())
            {//Si no existe, sumar todo el kilometraje final e inicial y restar sus resultados.
                kmRecorrido = kms.Sum(k => k.KilometrajeFinal - k.KilometrajeInicial);
            }
            else
            {//Si existe, obtener el ultimo registro de mantención para obtener el kilometraje del vehículo
             //y este restarlo a la suma de los kilometrajes  
                var lastFM = fichaMant.LastOrDefault(fm => fm.IdVehiculo == id);
                kmRecorrido = lastFM.Kilometraje - kms.Where(k => k.FechaCreacion >= lastFM.FechaMantencion).Sum(k => k.KilometrajeFinal);
            }
            return periodo >= kmRecorrido; //Devuelve true si el periodo seleccionado para el vehículo es mayor que los kilómetros recorridos
        }
        /// <summary>
        /// Obtiene una lista de vehículos para su selección.
        /// </summary>
        /// <returns>Una lista que contiene los vehículos con sus respectivas categorías.</returns>
        private async Task<List<SelectListWithGroups>> ObtenerVehiculos()
        {
            var categorias = _context.Categorias.OrderBy(c => c.Categoria1).ToList();
            var sli = new List<SelectListItem>();
            if (User.IsInRole("Mantenedor de vehículos pesados") && User.IsInRole("Mantenedor de vehículos no pesados") || User.IsInRole("Administrador"))
            {//Obtener los vehículos asignado al rol "Mantenedor Vehículos Pesado", "Mantenedor de vehículos no pesados" y "Administrador"
                return await _context.Vehiculos.Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado)
                    .Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    }).ToListAsync();
            }
            else if (User.IsInRole("Mantenedor de vehículos no pesados") || User.IsInRole("Administrador"))
            {//Obtener los vehículos asignado al rol "Mantenedor de vehículos no pesados" y "Administrador"
                return await _context.Vehiculos.Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado)
                    .Where(v => v.IdCategoriaNavigation.Categoria1.Equals("SUV") || v.IdCategoriaNavigation.Categoria1.Equals("Camioneta")
                            || v.IdCategoriaNavigation.Categoria1.Equals("Auto") || v.IdCategoriaNavigation.Categoria1.Equals("Furgon"))
                    .Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    }).ToListAsync();
            }
            else
            {//Obtener los vehículos asignado al rol "Mantenedor de vehículos pesados" y "Administrador"
                return await _context.Vehiculos.Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado)
                    .Where(v => v.IdCategoriaNavigation.Categoria1.Equals("Camion") || v.IdCategoriaNavigation.Categoria1.Equals("Retroexcavadora")
                            || v.IdCategoriaNavigation.Categoria1.Equals("Bus") || v.IdCategoriaNavigation.Categoria1.Equals("Tractor"))
                    .Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    }).ToListAsync();
            }
        }
    }
}
