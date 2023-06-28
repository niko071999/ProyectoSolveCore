using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con los registros de la mantención 
    /// de los vehículos.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver, crear, modificar y eliminar 
    /// registros de la mantención de los vehículos.
    /// </remarks>
    public class MantencionController : Controller
    {
        /// <summary>
        /// contexto de la base de datos
        /// </summary>
        private readonly ModelData _context;
        //Constructor
        public MantencionController(ModelData context)
        {
            _context = context;
        }
        /// <summary>
        /// Método que muestra todas las mantenciones realizadas a los vehículos
        /// </summary>
        /// <returns>Un vista con todas las mantenciones realizadas</returns>
        [Autorizar(permisoId: 16)]
        public async Task<IActionResult> VisualizarMantenciones()
        {
            var mantenciones = await _context.Fichamantencions.Include(m => m.IdConductorNavigation.IdUsuarioNavigation).Include(m => m.IdVehiculoNavigation)
                .ToListAsync();
            return View(mantenciones);
        }
        /// <summary>
        /// Método que muestra un formulario para ingresar una entrada a la ficha de mantenciones
        /// </summary>
        /// <param name="id">ID del vehículo. Si es 0 es porque se selecciona en el formulario</param>
        /// <returns>Una vista con un formulario para ingresar una entrada a la ficha de la mantención</returns>
        [Autorizar(permisoId:14)]
        public async Task<IActionResult> AgregarEntradaFM(int id = -1)
        {
            if (id == -1)//Esto quiere decir que viene nulo el id
            {
                ViewBag.MensajeError = "Valor nulo";
                return RedirectToAction("VisualizarMantenciones");
            }
            try
            {
                //Lista de vehículo
                var vlist = new List<SelectListWithGroups>();

                var vehiculos = await _context.Vehiculos.Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado)
                    .ToListAsync();
                //Verifica si el usuario es mantenedor de vehículos pesados o no pesados,
                //para mostrar solo las categorías de vehículos correspondiente para el rol
                if (User.IsInRole("Mantenedor de vehículos no pesados") && User.IsInRole("Mantenedor de vehículos pesados") || User.IsInRole("Administrador"))
                {
                    vlist = vehiculos.Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    }).ToList();
                }else if (User.IsInRole("Mantenedor de vehículos no pesados"))  
                {
                    vlist = vehiculos.Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    })
                    .Where(vl => vl.Group.Equals("SUV") || vl.Group.Equals("Camioneta")
                    || vl.Group.Equals("Auto") || vl.Group.Equals("Furgon"))
                    .ToList();
                }
                else
                {
                    vlist = vehiculos.Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    })
                    .Where(v => v.Group.Equals("Camion") || v.Group.Equals("Retroexcavadora")
                            || v.Group.Equals("Bus") || v.Group.Equals("Tractor"))
                    .ToList();
                }
                //Lista de conductores
                var clist = await _context.Conductores.Include(c => c.IdUsuarioNavigation)
                    .Where(c => !c.Eliminado)
                    .Select(c => new SelectListWithGroups()
                    {
                        Value = c.Id.ToString(),
                        Text = $"{c.IdUsuarioNavigation.Nombre} {c.IdUsuarioNavigation.Apellido}"
                    }).ToListAsync();
                //Verificar valor del ID del vehículo
                if (id == 0)
                {//Si el ID del vehículo viene en 0, es porque se selecciona desde el formulario
                    var v = vehiculos.FirstOrDefault(v => int.Parse(vlist.FirstOrDefault().Value) == v.Id);
                    ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", null, "Group");
                    ViewBag.IdConductor = new SelectList(clist, "Value", "Text", v.IdConductor);
                }
                else
                {//Si el ID viene con un valor mayor a 0, es porque debe ir seleccionado para el usuario
                    var v = vehiculos.FirstOrDefault(v => v.Id == id);
                    //Verificar si el conductor existe, para seleccionarlo en la lista
                    if (v.IdConductor.HasValue)
                    {
                        ViewBag.IdConductor = new SelectList(clist, "Value", "Text", v.IdConductor);
                    }
                    else
                    {
                        ViewBag.IdConductor = new SelectList(clist, "Value", "Text");

                    }
                    ViewBag.IdVehiculo = new SelectList(vlist, "Value", "Text", id, "Group");
                }
                var vehiculo = vehiculos.FirstOrDefault();
                if (vehiculo.IdConductor.HasValue)
                {
                    ViewBag.IdConductor = new SelectList(clist, "Value", "Text", vehiculo.IdConductor);
                }
                else
                {
                    ViewBag.IdConductor = new SelectList(clist, "Value", "Text");
                }
                return View(new vmFichaMantencion());
            }
            catch (Exception ex)
            {
                ViewBag.MensajeError = "Error: " + ex.Message;
                return RedirectToAction("VisualizarMantenciones");
            }
        }
        /// <summary>
        /// Método el cual agrega la entrada a la ficha de mantención
        /// </summary>
        /// <param name="fm">Entrada de la ficha de mantención del vehículo</param>
        /// <returns>Un re direccionamiento a la vista correspondiente, según el resultado obtenido</returns>
        [Autorizar(permisoId: 14)]
        [HttpPost]
        public async Task<IActionResult> AgregarEntradaFM(vmFichaMantencion fm)
        {
            if (fm == null)
            {
                return View(new vmFichaMantencion());
            }
            try
            {
                //Se inicia la transacción de a la base de datos
                using var transaction = await _context.Database.BeginTransactionAsync();
                Fichamantencion ficha = new()
                {
                    FechaMantencion = fm.FechaMantencion,
                    Kilometraje = fm.Kilometraje,
                    Descripcion = fm.Descripcion,
                    IdConductor = fm.IdConductor,
                    IdVehiculo = fm.IdVehiculo
                };
                await _context.Fichamantencions.AddAsync(ficha);
                await _context.SaveChangesAsync();

                var v = await _context.Vehiculos.FindAsync(fm.IdVehiculo);
                if (v == null) return View(fm);
                if (!v.Estado)
                {
                    v.Estado = true;
                    await _context.SaveChangesAsync();
                }
                // Se realiza Comité de la transacción de la base de datos
                // para finalizarla y guardar los cambios
                await transaction.CommitAsync();
                return RedirectToAction("VisualizarMantenciones");
            }
            catch (Exception ex)
            {
                return RedirectToAction("VisualizarMantenciones");
            }
        }
    }
}
