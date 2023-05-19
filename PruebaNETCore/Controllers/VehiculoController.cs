using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;
using System.Collections;

namespace ProyectoSolveCore.Controllers
{
    [Authorize(Roles = "Administrador, Mantenedor de vehículos")]
    public class VehiculoController : Controller
    {
        private readonly ModelData _context;
        public VehiculoController(ModelData context)
        {
            _context = context;
        }
        [Authorize(Roles = "Administrador, Mantenedor de vehículos, Jefe")]
        public async Task<IActionResult> VisualizarVehiculos()
        {
            try
            {
                // Obtiene la lista de vehículos con sus respectivos kilometrajes
                var vehiculos = await _context.Vehiculos.Include(v => v.IdPeriodoKilometrajeNavigation).Include(v => v.Kilometrajes)
                    .Where(v => !v.Eliminado).ToListAsync();
                var viewViehiculos = vehiculos.Select(v => new vmVehiculo()
                {
                    Id = v.Id,
                    Patente = v.Patente,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Habilitado = v.Estado ? "SI" : "NO",
                    Km_Recorrido = !v.Kilometrajes.Any() ? decimal.Zero : v.Kilometrajes.FirstOrDefault().KilometrajeInicial + 
                    v.Kilometrajes.Max(k => k.KilometrajeFinal) - v.Kilometrajes.Min(k => k.KilometrajeInicial)
                }).ToList();

                // Retorna la vista con la lista de vehículos
                return View(viewViehiculos);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        public IActionResult AgregarVehiculo()
        {
            ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
            return View(new vmVehiculoKm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarVehiculo(vmVehiculoKm v)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                if (v == null)
                {
                    ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
                    return View(v);
                }
                var kilometrajeStr = $"{v.KilometrajeInicialEntero}.{v.KilometrajeInicialDecimal}";
                if (!decimal.TryParse(kilometrajeStr, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal kilometraje))
                {
                    ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
                    return View(v);
                }

                var vehiculo = new Vehiculo()
                {
                    Id = v.Id,
                    Patente = v.Patente,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Eliminado = false,
                    Estado = v.Estado,
                    IdPeriodoKilometraje = v.IdPeriodoKilometraje,
                };
                _context.Vehiculos.Add(vehiculo);
                int n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
                    return View(v);
                }

                var k = new Kilometraje()
                {
                    IdVehiculo = vehiculo.Id,
                    KilometrajeInicial = kilometraje,
                    KilometrajeFinal = kilometraje,

                };
                _context.Add(k);
                n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
                    return View(v);
                }
                await transaction.CommitAsync();
                return RedirectToAction("VisualizarVehiculos");
            }
            catch (Exception)
            {
                ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
                return View(v);
            }
        }
        public async Task<PartialViewResult> EditarVehiculo(int id = 0)
        {
            try
            {
                string mensaje;
                if (id == 0)
                {
                    mensaje = "Hubo un error al recibir los datos.";
                    return PartialView("_PartialModalError", mensaje);
                }
                var vehiculo = await _context.Vehiculos.FindAsync(id);
                if (vehiculo == null)
                {
                    mensaje = "Hubo un error al recibir los datos.";
                    return PartialView("_PartialModalError", mensaje);
                }
                var periodos = await _context.PeriodosMantenimientos.Select(p => new vmPeriodosMantenimiento()
                {
                    id = p.Id,
                    periodo = 0 < p.PeriodoKilometraje - Math.Floor(p.PeriodoKilometraje) ?
                        "Cada "+p.PeriodoKilometraje.ToString("N") + " Km"
                        : "Cada " + p.PeriodoKilometraje.ToString("N0") + " Km",
                }).ToListAsync();
                ViewBag.IdPeriodoKilometraje = new SelectList(periodos, "id", "periodo", vehiculo.IdPeriodoKilometraje);
                return PartialView("_EditarVehiculo", vehiculo);
            }
            catch (Exception)
            {
                string mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditarVehiculo(Vehiculo v)
        {
            try
            {
                if (v == null)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrio un error al recibir los datos",
                        type = "error"
                    });
                }
                _context.Update(v).State = EntityState.Modified;
                int n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrio un error al guardar los datos",
                        type = "error"
                    });
                }
                return Json(new
                {
                    mensaje = "Vehículo editado correctamente",
                    type = "success"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente",
                    type = "danger"
                });
            }
        }
        public async Task<PartialViewResult> EliminarVehiculo(int id = 0)
        {
            try
            {
                string mensaje;
                if (id == 0)
                {
                    mensaje = "Hubo un error al recibir los datos, intentelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }
                var vehiculo = await _context.Vehiculos.FindAsync(id);
                if (vehiculo == null)
                {
                    mensaje = "Hubo un error al recibir obtener los datos, intentelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }

                return PartialView("_BorrarVehiculo", vehiculo);
            }
            catch (Exception)
            {
                string mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> BorrarVehiculo(int id_vehiculo = 0)
        {
            try
            {
                if (id_vehiculo == 0)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrio un error al recibir los datos",
                        type = "error"
                    });
                }
                var vehiculo = await _context.Vehiculos.FindAsync(id_vehiculo);
                if (vehiculo == null)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrio un error al obtener los datos",
                        type = "error"
                    });
                }
                vehiculo.Eliminado = true;
                int n = await _context.SaveChangesAsync();

                if (n == 0)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrio un error al guardar los datos",
                        type = "error"
                    });
                }
                return Json(new
                {
                    mensaje = "Vehículo eliminado correctamente",
                    type = "success"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente",
                    type = "danger"
                });
            }
        }
        private IEnumerable GetPeriodosMantencion()
        {
            var periodos = _context.PeriodosMantenimientos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = 0 < p.PeriodoKilometraje - Math.Floor(p.PeriodoKilometraje) ?
                    "Cada " + p.PeriodoKilometraje.ToString("N") + " Km"
                    : "Cada " + p.PeriodoKilometraje.ToString("N0") + " Km",
            });
            return periodos;
        }
    }
}
