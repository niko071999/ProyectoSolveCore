using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;
using ProyectoSolveCore.Models.ViewModelsFilter;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace ProyectoSolveCore.Controllers
{
    [Authorize]
    public class VehiculoController : Controller
    {
        private readonly ModelData _context;
        public VehiculoController(ModelData context)
        {
            _context = context;
        }
        [Authorize(Roles = "Administrador, Mantenedor de vehículos no pesados, Mantenedor de vehículos pesados, Jefe")]
        public async Task<IActionResult> VisualizarVehiculos()
        {
            try
            {
                if (!await _context.Vehiculos.AnyAsync())
                {
                    return View(new List<vmVehiculo>());
                }

                // Obtiene la lista de vehículos con sus respectivos kilometrajes
                var vehiculos = await _context.Vehiculos.Include(v => v.IdPeriodoKilometrajeNavigation).Include(v => v.Kilometrajes)
                    .Include(v => v.IdCategoriaNavigation).Include(v => v.IdConductorNavigation.IdUsuarioNavigation)
                    .Where(v => !v.Eliminado).ToListAsync();
                var viewVehiculos = vehiculos.Select(v => new vmVehiculo()
                {
                    Id = v.Id,
                    Patente = v.Patente,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    IdCategoria = (int)v.IdCategoria,
                    Year = v.Year,
                    NombreConductor =  v.IdConductorNavigation != null 
                    ? $"<b>{v.IdConductorNavigation.IdUsuarioNavigation.Nombre} {v.IdConductorNavigation.IdUsuarioNavigation.Apellido}</b>" : "Sin conductor asignado",
                    Estado = v.Estado ? 1:0,
                    Km_Recorrido = !v.Kilometrajes.Any() ? 0 : v.Kilometrajes.FirstOrDefault().KilometrajeInicial + 
                    v.Kilometrajes.Max(k => k.KilometrajeFinal) - v.Kilometrajes.Min(k => k.KilometrajeInicial)
                }).ToList();

                viewVehiculos = await VerificarKmVehiculo(viewVehiculos);

                ViewBag.Patente = new SelectList(GetPatentes(viewVehiculos), "Value", "Text");
                ViewBag.Marca = new SelectList(GetMarcas(viewVehiculos), "Value", "Text");
                ViewBag.Modelo = new SelectList(GetModelo(viewVehiculos), "Value", "Text");
                ViewBag.IdCategoria = new SelectList(GetCategorias(vehiculos), "Value", "Text");
                ViewBag.OpcionEstado = 0;

                // Retorna la vista con la lista de vehículos
                return View(viewVehiculos);
            }
            catch (Exception ex)
            {
                return View(new List<vmVehiculo>()); ;
            }
        }
        [HttpPost]
        public async Task<IActionResult> VisualizarVehiculos(vmFiltrosVehiculos fv)
        {
            if (fv == null)
            {
                return View(new List<vmVehiculo>());
            }

            var vehiculos = await _context.Vehiculos.Include(v => v.IdPeriodoKilometrajeNavigation).Include(v => v.Kilometrajes).Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado).ToListAsync();
            try
            {

                var viewVehiculos = vehiculos.Select(v => new vmVehiculo()
                {
                    Id = v.Id,
                    Patente = v.Patente,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Year = v.Year,
                    NombreConductor = $"{v.IdConductorNavigation.IdUsuarioNavigation.Nombre} {v.IdConductorNavigation.IdUsuarioNavigation.Apellido}",
                    IdCategoria = (int)v.IdCategoria,
                    Estado = v.Estado ? 1 : 0,
                    Km_Recorrido = !v.Kilometrajes.Any() ? 0 : v.Kilometrajes.FirstOrDefault().KilometrajeInicial +
                    v.Kilometrajes.Max(k => k.KilometrajeFinal) - v.Kilometrajes.Min(k => k.KilometrajeInicial)
                }).ToList();

                viewVehiculos = await VerificarKmVehiculo(viewVehiculos);
                
                if (fv.IdCategoria == 0 && string.IsNullOrEmpty(fv.Marca) && string.IsNullOrEmpty(fv.Modelo)
                    && string.IsNullOrEmpty(fv.Patente) && fv.OpcionEstado == 0)
                {
                    return View(viewVehiculos);
                }
                List<vmVehiculo> NewList = new(viewVehiculos);
                if (fv.IdCategoria > 0)
                {
                    NewList = NewList.Where(v => v.IdCategoria == fv.IdCategoria).ToList();
                }
                if (!string.IsNullOrEmpty(fv.Marca))
                {
                    NewList = NewList.Where(v => v.Marca.Equals(fv.Marca)).ToList();
                }
                if (!string.IsNullOrEmpty(fv.Modelo))
                {
                    NewList = NewList.Where(v => v.Modelo.Equals(fv.Modelo)).ToList();
                }
                if (!string.IsNullOrEmpty(fv.Patente))
                {
                    NewList = NewList.Where(v => v.Patente.Equals(fv.Patente)).ToList();
                }
                if (fv.OpcionEstado > 0)
                {
                    NewList = NewList.Where(v => v.Estado == fv.OpcionEstado).ToList();
                }

                ViewBag.Patente = new SelectList(GetPatentes(NewList), "Value", "Text", fv.Patente);
                ViewBag.Marca = new SelectList(GetMarcas(NewList), "Value", "Text", fv.Marca);
                ViewBag.Modelo = new SelectList(GetModelo(NewList), "Value", "Text", fv.Modelo);
                ViewBag.IdCategoria = new SelectList(GetCategorias(vehiculos), "Value", "Text", fv.IdCategoria);
                ViewBag.OpcionEstado = fv.OpcionEstado;
                return View(NewList);
            }
            catch (Exception ex)
            {
                var viewVehiculos = vehiculos.Select(v => new vmVehiculo()
                {
                    Id = v.Id,
                    Patente = v.Patente,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Year = v.Year,
                    IdCategoria = (int)v.IdCategoria,
                    Estado = v.Estado ? 1 : 0,
                    Km_Recorrido = !v.Kilometrajes.Any() ? 0 : v.Kilometrajes.FirstOrDefault().KilometrajeInicial +
                    v.Kilometrajes.Max(k => k.KilometrajeFinal) - v.Kilometrajes.Min(k => k.KilometrajeInicial)
                }).ToList();
                viewVehiculos = await VerificarKmVehiculo(viewVehiculos);
                return View(viewVehiculos);
            }

        }
        /// <summary>
        /// Verifica el kilometraje y modifica el estado de los vehículos. Asigna un mensaje de estado correspondiente a cada vehículo.
        
        /// <param name="viewViehiculos">Lista de vehículos a verificar.</param>
        /// <returns>Una lista de objetos vmVehiculo con los mensajes de estado asignados.</returns>
        private async Task<List<vmVehiculo>> VerificarKmVehiculo(List<vmVehiculo> viewViehiculos)
        {
            try
            {
                List<vmVehiculo> newList = new();
                var fichaMants = await _context.Fichamantencions.ToListAsync();
                var kms = await _context.Kilometrajes.Include(k => k.IdVehiculoNavigation.Fichamantencions)
                        .ToListAsync();
                var vehiculoIds = viewViehiculos.Select(v => v.Id).ToList();
                var periodos = await _context.Vehiculos.Include(v => v.IdPeriodoKilometrajeNavigation)
                    .Where(v => vehiculoIds.Contains(v.Id)).ToListAsync();

                foreach (var v in viewViehiculos)
                {
                    if (v.Estado == 1 || v.Estado == 2)
                    {
                        var periodo = periodos.FirstOrDefault(p => p.Id == v.Id)?.IdPeriodoKilometrajeNavigation?.PeriodoKilometraje ?? 0;
                        var vehiculoKms = kms.Where(k => k.IdVehiculo == v.Id).ToList();
                        var vehiculoFichaMant = fichaMants.LastOrDefault(f => f.IdVehiculo == v.Id);

                        int kmRecorrido;
                        if (vehiculoFichaMant == null)
                        {
                            kmRecorrido = vehiculoKms.Sum(k => k.KilometrajeFinal - k.KilometrajeInicial);
                        }
                        else
                        {
                            kmRecorrido = vehiculoFichaMant.Kilometraje - vehiculoKms
                                .Sum(k => k.KilometrajeFinal);
                        }

                        if (kmRecorrido == 0)
                        {
                            v.MensajeEstado = "El vehiculo se encuentra en 0 Kilometros";
                            newList.Add(v);
                            continue;
                        }

                        double porc = ((double)kmRecorrido / periodo) * 100;
                        if (periodo <= kmRecorrido)
                        {
                            v.MensajeEstado = "El vehiculo necesita obligatoriamente mantencion";
                            v.Estado = 0; //Estado: Desahabilitado
                            var vehiculo = await _context.Vehiculos.FindAsync(v.Id);
                            if (vehiculo != null && vehiculo.Estado)
                            {
                                vehiculo.Estado = false;
                                await _context.SaveChangesAsync();
                            }
                            newList.Add(v);
                            continue;
                        }

                        if (porc > 50)
                        {
                            var diferencia = periodo - kmRecorrido;
                            v.Estado = 2;//Estado: Advertencia
                            v.MensajeEstado = $"Al vehículo le faltan {diferencia} km para hacerle mantencion";
                            newList.Add(v);
                            continue;
                        }

                        v.MensajeEstado = "El vehículo esta en perfecto estado";
                        newList.Add(v);
                    }
                    else
                    {
                        newList.Add(v);
                    }
                }
                return newList;
            }
            catch (Exception ex)
            {
                return new List<vmVehiculo>();
            }
        }

        public IActionResult AgregarVehiculo()
        {
            ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
            ViewBag.IdCategoria = new SelectList(GetCategorias(), "Value", "Text");
            ViewBag.IdConductor = new SelectList(GetConductores(), "Value", "Text");
            return View(new vmVehiculoKm());
        }
        [HttpPost]
        public async Task<IActionResult> AgregarVehiculo(vmVehiculoKm v)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                if (v == null)
                {
                    ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
                    ViewBag.IdCategoria = new SelectList(GetCategorias(), "Value", "Text");
                    ViewBag.IdConductor = new SelectList(GetConductores(), "Value", "Text");
                    return View(new vmVehiculo());
                }

                bool exist = await _context.Vehiculos.AnyAsync(x => string.Equals(x.Patente.ToUpper(), v.Patente.ToUpper().Trim()) 
                    && !x.Eliminado);

                if (exist)
                {
                    ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text", v.IdPeriodoKilometraje);
                    ViewBag.IdCategoria = new SelectList(GetCategorias(), "Value", "Text", v.IdCategoria);
                    return View(v);
                }

                var vehiculo = new Vehiculo()
                {
                    Id = v.Id,
                    Patente = v.Patente,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Year = v.Year,
                    Eliminado = false,
                    Estado = v.Estado,
                    IdPeriodoKilometraje = v.IdPeriodoKilometraje,
                    IdCategoria = v.IdCategoria,
                    IdConductor = v.IdConductor
                };
                await _context.Vehiculos.AddAsync(vehiculo);
                int n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
                    ViewBag.IdCategoria = new SelectList(GetCategorias(), "Value", "Text");
                    return View(v);
                }

                var k = new Kilometraje()
                {
                    IdVehiculo = vehiculo.Id,
                    FechaCreacion = GenerarFecha(DateTime.Now),
                    KilometrajeInicial = v.KilometrajeInicial,
                    KilometrajeFinal = v.KilometrajeInicial,
                };
                await _context.AddAsync(k);
                n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
                    ViewBag.IdCategoria = new SelectList(GetCategorias(), "Value", "Text");
                    return View(v);
                }
                await transaction.CommitAsync();
                return RedirectToAction("VisualizarVehiculos");
            }
            catch (Exception ex)
            {
                ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text");
                ViewBag.IdCategoria = new SelectList(GetCategorias(), "Value", "Text");
                return View(v);
            }
        }

        private DateTime GenerarFecha(DateTime now)
        {
            var hoystr = now.ToString("dd-MM-yyyy HH:mm:ss");
            return DateTime.ParseExact(hoystr, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
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
                ViewBag.IdPeriodoKilometraje = new SelectList(GetPeriodosMantencion(), "Value", "Text", vehiculo.IdPeriodoKilometraje);
                ViewBag.IdCategoria = new SelectList(GetCategorias(), "Value", "Text", vehiculo.IdCategoria);
                ViewBag.IdConductor = new SelectList(GetConductores(), "Value", "Text", vehiculo.IdConductor??null);
                return PartialView("_EditarVehiculo", vehiculo);
            }
            catch (Exception)
            {
                string mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }
        [HttpPost]
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

                if (await _context.Vehiculos.AnyAsync(x => x.IdConductor == v.IdConductor))
                {
                    return Json(new
                    {
                        mensaje = "El conductor ya fue asignado a otro vehiculo",
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
        public async Task<JsonResult> SelectAgregarPeriodo(int periodo = 0)
        {
            if (periodo == 0)
            {
                return Json(new
                {
                    data = "",
                    type = "void",
                    mensaje = "Ocurrio un error al recibir los datos"
                });
            }
            if (await _context.Periodosmantenimientos.AnyAsync(x => x.PeriodoKilometraje == periodo))
            {
                return Json(new
                {
                    data = "",
                    type = "error",
                    mensaje = "Ocurrio un error al verificar los periodos, ya existe un registro con este valor"
                });
            }

            var p = new Periodosmantenimiento()
            {
                PeriodoKilometraje = periodo,
            };
            await _context.Periodosmantenimientos.AddAsync(p);
            int n = await _context.SaveChangesAsync();
            if (n == 0)
            {
                return Json(new
                {
                    data = "",
                    type = "error",
                    mensaje = "Ocurrio un error al guardar los cambios en la base de datos"
                });
            }

            return Json(new
            {
                data = new
                {
                    Id = p.Id,
                    Periodo = "Cada " + p.PeriodoKilometraje.ToString("N0") + " Km"
                },
                type = "success",
                mensaje = "Se a agregado el periodo existosamente"
            });
        }
        public async Task<JsonResult> SelectAgregarCategoria(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
            {
                return Json(new
                {
                    data = "",
                    type = "void",
                    mensaje = "Ocurrio un error al recibir los datos"
                });
            }
            if (await _context.Categorias.AnyAsync(x => x.Categoria1.Equals(categoria)))
            {
                return Json(new
                {
                    data = "",
                    type = "error",
                    mensaje = "Ocurrio un error al verificar las categorias, ya existe un registro con este valor"
                });
            }

            var c= new Categoria()
            {
                Categoria1 = categoria,
            };
            await _context.Categorias.AddAsync(c);
            int n = await _context.SaveChangesAsync();
            if (n == 0)
            {
                return Json(new
                {
                    data = "",
                    type = "error",
                    mensaje = "Ocurrio un error al guardar los cambios en la base de datos"
                });
            }

            return Json(new
            {
                data = c,
                type = "success",
                mensaje = "Se a agregado la categoría existosamente"
            });
        }
        private List<SelectListItem> GetPeriodosMantencion()
        {
            var periodos = _context.Periodosmantenimientos.OrderBy(p => p.PeriodoKilometraje).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = "Cada " + p.PeriodoKilometraje.ToString("N0") + " Km"
            }).ToList();
            return periodos;
        }
        private static List<SelectListItem> GetCategorias(List<Vehiculo> vehiculos)
        {
            List<SelectListItem> list = new();
            var IdsCategorias = vehiculos.Select(v => v.IdCategoria).Distinct().ToList();

            foreach (var id in IdsCategorias)
            {
                if (vehiculos.Any(v => v.IdCategoria == id))
                {
                    var v = vehiculos.FirstOrDefault(v => v.IdCategoria == id);
                    list.Add(new SelectListItem
                    {
                        Value = id.ToString(),
                        Text = v.IdCategoriaNavigation.Categoria1
                    });
                }
            }
            return list.OrderBy(c => c.Text).ToList();
        }

        private static List<SelectListItem> GetModelo(List<vmVehiculo> viewViehiculos)
        {
            List<SelectListItem> list = new();
            var modelos = viewViehiculos.Select(v => v.Modelo).Distinct().ToList();

            foreach (var m in modelos)
            {
                if (viewViehiculos.Any(v => v.Modelo.Equals(m)))
                {
                    list.Add(new SelectListItem
                    {
                        Value = m,
                        Text = m
                    });
                }
            }
            return list.OrderBy(m => m.Text).ToList();
        }

        private static List<SelectListItem> GetMarcas(List<vmVehiculo> viewViehiculos)
        {
            List<SelectListItem> list = new();
            var marcas = viewViehiculos.Select(v => v.Marca).Distinct().ToList();

            foreach (var m in marcas)
            {
                if (viewViehiculos.Any(v => v.Marca.Equals(m)))
                {
                    list.Add(new SelectListItem
                    {
                        Value = m,
                        Text = m
                    });
                }
            }
            return list.OrderBy(m => m.Text).ToList();
        }

        private static List<SelectListItem> GetPatentes(List<vmVehiculo> viewViehiculos)
        {
            List<SelectListItem> list = new();
            var patentes = viewViehiculos.Select(v => v.Patente).Distinct().ToList();

            foreach (var p in patentes)
            {
                if (viewViehiculos.Any(v => v.Patente.Equals(p)))
                {
                    list.Add(new SelectListItem
                    {
                        Value = p,
                        Text = p
                    });
                }

            }

            return list.OrderBy(p => p.Text).ToList();
        }
        private IEnumerable GetCategorias()
        {
            var categorias = _context.Categorias.OrderBy(c => c.Categoria1).ToList();

            if (User.IsInRole("Mantenedor de vehículos pesados") && User.IsInRole("Mantenedor de vehículos no pesados"))
            {
                return categorias.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Categoria1
                });
            }else if (User.IsInRole("Mantenedor de vehículos no pesados"))
            {
                return categorias.Where(c => c.Categoria1.Equals("SUV") || c.Categoria1.Equals("Camioneta")
                || c.Categoria1.Equals("Auto") || c.Categoria1.Equals("Furgon"))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Categoria1
                });
            }
            else
            {
                return categorias.Where(c => c.Categoria1.Equals("Camion") || c.Categoria1.Equals("Retroexcavadora")
                || c.Categoria1.Equals("Bus") || c.Categoria1.Equals("Tractor"))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Categoria1
                });
            }
        }
        private List<SelectListItem> GetConductores()
        {
            DateTime hoy = GenerarFecha(DateTime.Now);
            return _context.Conductores.OrderBy(c => c.IdUsuarioNavigation.Nombre).Where(c => c.FechaVencimiento >= hoy)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.IdUsuarioNavigation.Nombre} {c.IdUsuarioNavigation.Apellido}"
                }).ToList();
        }
        public async Task<JsonResult> GetConductorVehiculo(int id = 0)
        {
            int idconductor = 0;
            if (id == 0)
            {
                return Json(idconductor);
            }
            var vehiculo = await _context.Vehiculos.FirstOrDefaultAsync(v => v.Id == id);
            if (vehiculo.IdConductor.HasValue)
            {
                idconductor = (int)vehiculo.IdConductor;
                return Json(idconductor);
            }
            return Json(idconductor);
        }
    }
}
