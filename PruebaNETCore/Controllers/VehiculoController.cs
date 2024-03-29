﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;
using ProyectoSolveCore.Models.ViewModelsFilter;
using System.Collections;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con los vehículos del sistema.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver, crear, modificar y eliminar 
    /// los vehículos del sistema.
    /// </remarks>
    [Authorize]
    public class VehiculoController : Controller
    {
        /// <summary>
        /// Contexto de la base de datos
        /// </summary>
        private readonly ModelData _context;
        //Constructor
        public VehiculoController(ModelData context)
        {
            _context = context;
        }
        /// <summary>
        /// Método que muestra una vista con la lista de todos los vehículos.
        /// </summary>
        /// <returns>Una vista que representa la vista con la lista de vehículos.</returns>
        [Autorizar(permisoId:6)]
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
                //Obtenemos la cantidad de vehículos en cada estado
                int habilitados = viewVehiculos.Where(v => v.Estado == 1).Count();
                int deshabilitados = viewVehiculos.Where(v => v.Estado == 0).Count();
                int advertencias = viewVehiculos.Count - (habilitados + deshabilitados);
                //Preparar los ViewBag para el filtro
                ViewBag.Patente = new SelectList(GetPatentes(viewVehiculos), "Value", "Text");
                ViewBag.Marca = new SelectList(GetMarcas(viewVehiculos), "Value", "Text");
                ViewBag.Modelo = new SelectList(GetModelo(viewVehiculos), "Value", "Text");
                ViewBag.IdCategoria = new SelectList(GetCategorias(vehiculos), "Value", "Text");
                ViewBag.Habilitados = habilitados;
                ViewBag.Deshabilitados = deshabilitados;
                ViewBag.Advertencias = advertencias;
                ViewBag.OpcionEstado = 0;

                // Retorna la vista con la lista de vehículos
                return View(viewVehiculos);
            }
            catch (Exception ex)
            {
                return View(new List<vmVehiculo>()); ;
            }
        }
        /// <summary>
        /// Método que muestra una vista con la lista de vehículos filtrada según los parámetros proporcionados.
        /// </summary>
        /// <param name="fv">Contiene los filtros para la búsqueda de vehículos.</param>
        /// <returns>La vista con la lista de vehículos filtrada.</returns>
        [Autorizar(permisoId:6)]
        [HttpPost]
        public async Task<IActionResult> VisualizarVehiculos(vmFiltrosVehiculos fv)
        {
            if (fv == null)
            {
                return View(new List<vmVehiculo>());
            }
            // Obtiene la lista de vehículos sin filtros de búsqueda
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
                    NombreConductor = v.IdConductorNavigation != null
                        ? $"<b>{v.IdConductorNavigation.IdUsuarioNavigation.Nombre} {v.IdConductorNavigation.IdUsuarioNavigation.Apellido}</b>" 
                        : "Sin conductor asignado",
                    IdCategoria = (int)v.IdCategoria,
                    Estado = v.Estado ? 1 : 0,
                    Km_Recorrido = !v.Kilometrajes.Any() ? 0 : v.Kilometrajes.FirstOrDefault().KilometrajeInicial +
                    v.Kilometrajes.Max(k => k.KilometrajeFinal) - v.Kilometrajes.Min(k => k.KilometrajeInicial)
                }).ToList();
                // Verifica los kilómetros de los vehículos
                viewVehiculos = await VerificarKmVehiculo(viewVehiculos);
                // Verifica si no se aplicaron filtros y devuelve la vista con la lista de vehículos sin cambios
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
                // Calcula la cantidad de vehículos habilitados, deshabilitados y con advertencias
                int habilitados = NewList.Where(v => v.Estado == 1).Count();
                int deshabilitados = NewList.Where(v => v.Estado == 0).Count();
                int advertencias = (habilitados + deshabilitados) - NewList.Count;
                // Configura las variables ViewBag para pasar datos a la vista
                ViewBag.Patente = new SelectList(GetPatentes(NewList), "Value", "Text", fv.Patente);
                ViewBag.Marca = new SelectList(GetMarcas(NewList), "Value", "Text", fv.Marca);
                ViewBag.Modelo = new SelectList(GetModelo(NewList), "Value", "Text", fv.Modelo);
                ViewBag.IdCategoria = new SelectList(GetCategorias(vehiculos), "Value", "Text", fv.IdCategoria);
                ViewBag.OpcionEstado = fv.OpcionEstado;
                ViewBag.Habilitados = habilitados;
                ViewBag.Deshabilitados = deshabilitados;
                ViewBag.Advertencias = advertencias;
                return View(NewList);
            }
            catch (Exception ex)
            { // En caso de error, devuelve la vista con la lista de vehículos sin cambios
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
                //Nueva lista de vehículos con sus estados actualizados que se retorna a la vista
                List<vmVehiculo> newList = new();
                //Mantenciones
                var fichaMants = await _context.Fichamantencions.ToListAsync();
                //Historial de kilometrajes
                var kms = await _context.Kilometrajes.Include(k => k.IdVehiculoNavigation.Fichamantencions)
                        .ToListAsync();
                //ID's de los vehículos
                var vehiculoIds = viewViehiculos.Select(v => v.Id).ToList();
                //Periodos de mantención de los vehículos
                var periodos = await _context.Vehiculos.Include(v => v.IdPeriodoKilometrajeNavigation)
                    .Where(v => vehiculoIds.Contains(v.Id)).ToListAsync();

                foreach (var v in viewViehiculos)
                {
                    //Verificar estado del vehículo
                    if (v.Estado == 1 || v.Estado == 2)
                    {//Si es 1 o 2, verificar los km de los vehículos
                        var periodo = periodos.FirstOrDefault(p => p.Id == v.Id)?.IdPeriodoKilometrajeNavigation?.PeriodoKilometraje ?? 0;
                        var vehiculoKms = kms.Where(k => k.IdVehiculo == v.Id).ToList();
                        //Recuperar el ultimo registro de mantención realizado al vehículo
                        var vehiculoFichaMant = fichaMants.LastOrDefault(f => f.IdVehiculo == v.Id);

                        int kmRecorrido;
                        //Se verifican si existe mantenciones al vehículo
                        if (vehiculoFichaMant == null)
                        {//Si no existen, se suman los kilometrajes
                            kmRecorrido = vehiculoKms.Sum(k => k.KilometrajeFinal - k.KilometrajeInicial);
                        }
                        else
                        {//Si no, se debe suman los kilometrajes y se resta con el ultimo kilometraje ingresado en la mantención
                            kmRecorrido = vehiculoFichaMant.Kilometraje - vehiculoKms
                                .Sum(k => k.KilometrajeFinal);
                        }
                        //Si esta en 0 km es por que es un vehículo nuevo o
                        //no se ha ingresado registros o con un o mas registros de mantenciones
                        if (kmRecorrido == 0)
                        {
                            if (vehiculoFichaMant != null)
                            {
                                v.MensajeEstado = "El vehículo esta habilitado y en perfecto estado";
                                newList.Add(v);
                                continue;
                            }
                            v.MensajeEstado = "El vehículo se encuentra en 0 Kilómetros";
                            newList.Add(v);
                            continue;
                        }
                        //Se saca el porcentaje que existe entre lo recorrido con el periodo
                        double porc = ((double)kmRecorrido / periodo) * 100;
                        if (periodo <= kmRecorrido)
                        {//Si el periodo asignado es menor al km recorrido es por que el vehículo necesita mantención
                            v.MensajeEstado = "El vehículo necesita obligadamente mantención";
                            v.Estado = 0; //Estado: Deshabilitado
                            var vehiculo = await _context.Vehiculos.FindAsync(v.Id);
                            if (vehiculo.Estado)
                            {//Cambiar el estado si el vehículo esta habilitado
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
                            v.MensajeEstado = $"Al vehículo le faltan {diferencia} km para hacerle mantención";
                            newList.Add(v);
                            continue;
                        }

                        v.MensajeEstado = "El vehículo esta habilitado y en perfecto estado";
                        newList.Add(v);
                    }
                    else
                    {//Si no, es por esta deshabilitado y no se necesita verificación
                        v.MensajeEstado = "El vehículo esta deshabilitado y necesita obligadamente mantención";
                        newList.Add(v);
                    }
                }
                return newList;
            }
            catch (Exception ex)
            {//Si da error, retorno una lista vacía
                return new List<vmVehiculo>();
            }
        }
        /// <summary>
        /// Método que muestra una vista para agregar un nuevo vehículo.
        /// </summary>La vista para agregar un nuevo vehículo.</returns>
        [Autorizar(permisoId:7)]
        public async Task<IActionResult> AgregarVehiculo()
        {
            ViewBag.IdPeriodoKilometraje = new SelectList(await GetPeriodosMantencion(), "Value", "Text");
            ViewBag.IdCategoria = new SelectList(await GetCategorias(), "Value", "Text");
            ViewBag.IdConductor = new SelectList(await GetConductores(), "Value", "Text");
            return View(new vmVehiculoKm());
        }
        /// <summary>
        /// Método que realiza la acción de agregar un nuevo vehículo.
        /// </summary>
        /// <param name="v">Los datos del vehículo a agregar.</param>
        /// <returns>La vista "VisualizarVehiculos" si la operación se realiza con éxito.</returns>
        [Autorizar(permisoId:7)]
        [HttpPost]
        public async Task<IActionResult> AgregarVehiculo(vmVehiculoKm v)
        {
            try
            {
                //Se inicia la transacción a la base de datos
                using var transaction = await _context.Database.BeginTransactionAsync();
                if (v == null)
                {
                    ViewBag.IdPeriodoKilometraje = new SelectList(await GetPeriodosMantencion(), "Value", "Text", v.IdPeriodoKilometraje);
                    ViewBag.IdCategoria = new SelectList(await GetCategorias(), "Value", "Text", v.IdCategoria);
                    ViewBag.IdConductor = new SelectList(await GetConductores(), "Value", "Text", v.IdConductor);
                    return View(new vmVehiculo());
                }
                //Verifica si existe un vehiculo no eliminado con la misma patente
                bool exist = await _context.Vehiculos.AnyAsync(x => string.Equals(x.Patente.ToUpper(), v.Patente.ToUpper().Trim()) 
                    && !x.Eliminado);

                if (exist)
                {//Si existe devuelve a la vista
                    ViewBag.IdPeriodoKilometraje = new SelectList(await GetPeriodosMantencion(), "Value", "Text", v.IdPeriodoKilometraje);
                    ViewBag.IdCategoria = new SelectList(await GetCategorias(), "Value", "Text", v.IdCategoria);
                    ViewBag.IdConductor = new SelectList(await GetConductores(), "Value", "Text", v.IdConductor);
                    return View(v);
                }
                //Se crea el objeto para añadir el vehiculo
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
                    ViewBag.IdPeriodoKilometraje = new SelectList(await GetPeriodosMantencion(), "Value", "Text", v.IdPeriodoKilometraje);
                    ViewBag.IdCategoria = new SelectList(await GetCategorias(), "Value", "Text", v.IdCategoria);
                    ViewBag.IdConductor = new SelectList(await GetConductores(), "Value", "Text", v.IdConductor);
                    return View(v);
                }
                //Se crea el objeto para añadir el kilometrjae
                var k = new Kilometraje()
                {
                    IdVehiculo = vehiculo.Id,
                    FechaCreacion = DateTime.Now,
                    KilometrajeInicial = v.KilometrajeInicial,
                    KilometrajeFinal = v.KilometrajeInicial,
                };
                await _context.AddAsync(k);
                n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    ViewBag.IdPeriodoKilometraje = new SelectList(await GetPeriodosMantencion(), "Value", "Text", v.IdPeriodoKilometraje);
                    ViewBag.IdCategoria = new SelectList(await GetCategorias(), "Value", "Text", v.IdCategoria);
                    ViewBag.IdConductor = new SelectList(await GetConductores(), "Value", "Text", v.IdConductor);
                    return View(v);
                }
                //Se realiza commit a la transaccion
                await transaction.CommitAsync();
                return RedirectToAction("VisualizarVehiculos");
            }
            catch (Exception ex)
            {
                ViewBag.IdPeriodoKilometraje = new SelectList(await GetPeriodosMantencion(), "Value", "Text", v.IdPeriodoKilometraje);
                    ViewBag.IdCategoria = new SelectList(await GetCategorias(), "Value", "Text", v.IdCategoria);
                    ViewBag.IdConductor = new SelectList(await GetConductores(), "Value", "Text", v.IdConductor);
                return View(v);
            }
        }
        /// <summary>
        /// Método que devuelve una vista parcial para la ventana emergente o modal donde se podrá editar los datos de un vehículo.
        /// </summary>
        /// <param name="id">ID del vehículo a editar.</param>
        /// <returns>Vista parcial que contiene el formulario para editar los datos del vehículo.</returns>
        [Autorizar(permisoId: 8)]
        public async Task<PartialViewResult> EditarVehiculo(int id = 0)
        {
            string mensaje;
            try
            {
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
                ViewBag.IdPeriodoKilometraje = new SelectList(await GetPeriodosMantencion(), "Value", "Text", vehiculo.IdPeriodoKilometraje);
                ViewBag.IdCategoria = new SelectList(await GetCategorias(), "Value", "Text", vehiculo.IdCategoria);
                ViewBag.IdConductor = new SelectList(await GetConductores(), "Value", "Text", vehiculo.IdConductor);
                return PartialView("_EditarVehiculo", vehiculo);
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error inesperado, consulte con administrador del sistema o inténtelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }

        public async Task<JsonResult> VerificarConductor(int? id)
        {
            bool ocupado = await _context.Vehiculos.AnyAsync(x => x.IdConductor == id && id != null);
            return Json(ocupado);
        }

        /// <summary>
        /// Método que se utiliza para guardar los cambios realizados en los datos de un vehículo.
        /// </summary>
        /// <param name="v">Contiene los datos editados del vehículo.</param>
        /// <returns>Un json con un mensaje indicando el resultado de la operación.</returns>
        [Autorizar(permisoId:8)]
        [HttpPost]
        public async Task<JsonResult> EditarVehiculo(Vehiculo v)
        {
            try
            {
                if (v == null)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrió un error al recibir los datos",
                        type = "error"
                    });
                }

                _context.Update(v).State = EntityState.Modified;
                int n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrió un error al guardar los datos",
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
                    mensaje = "Ocurrió un error inesperado, consulte con administrador del sistema o inténtelo nuevamente",
                    type = "danger"
                });
            }
        }
        /// <summary>
        /// Método que se utiliza para mostrar una vista parcial para una ventana emergente o modal donde se podrá confirmar la eliminación de un vehículo.
        /// </summary>
        /// <param name="id">ID del vehículo a eliminar.</param>
        /// <returns>vista parcial para confirmar eliminar vehículo.</returns>
        [Autorizar(permisoId:9)]
        public async Task<PartialViewResult> EliminarVehiculo(int id = 0)
        {
            string mensaje;
            try
            {
                if (id == 0)
                {
                    mensaje = "Hubo un error al recibir los datos, inténtelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }
                var vehiculo = await _context.Vehiculos.FindAsync(id);
                if (vehiculo == null)
                {
                    mensaje = "Hubo un error al recibir obtener los datos, inténtelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }

                return PartialView("_BorrarVehiculo", vehiculo);
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error inesperado, consulte con administrador del sistema o inténtelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }
        /// <summary>
        /// Método que se utiliza para eliminar un vehículo de la base de datos.
        /// </summary>
        /// <param name="id_vehiculo">ID del vehículo a eliminar.</param>
        /// <returns>Un json que contiene un mensaje como respuesta.</returns>
        [Autorizar(permisoId:9)]
        [HttpPost]
        public async Task<JsonResult> BorrarVehiculo(int id_vehiculo = 0)
        {
            try
            {
                if (id_vehiculo == 0)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrió un error al recibir los datos",
                        type = "error"
                    });
                }
                var vehiculo = await _context.Vehiculos.FindAsync(id_vehiculo);
                if (vehiculo == null)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrió un error al obtener los datos",
                        type = "error"
                    });
                }
                vehiculo.Eliminado = true;
                int n = await _context.SaveChangesAsync();

                if (n == 0)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrió un error al guardar los datos",
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
                    mensaje = "Ocurrió un error inesperado, consulte con administrador del sistema o inténtelo nuevamente",
                    type = "danger"
                });
            }
        }
        /// <summary>
        /// Método que se utiliza para agregar un nuevo periodo de mantenimiento a la base de datos.
        /// </summary>
        /// <param name="periodo">Valor del periodo de kilometraje.</param>
        /// <returns>Un json que contiene un mensaje y el periodo creado.</returns>
        public async Task<JsonResult> SelectAgregarPeriodo(int periodo = 0)
        {
            //Verificar si el periodo si no es un periodo
            if (double.IsNaN(periodo))
            {//Si no es un numero, crear un json con un mensaje de error
                return Json(new
                {
                    data = "",
                    type = "error",
                    mensaje = "El periodo debe ser un numero"
                });
            }
            //Verificar si el periodo es igual a 0
            if (periodo == 0)
            {
                return Json(new
                {
                    data = "",
                    type = "void",
                    mensaje = "Ocurrió un error al recibir los datos"
                });
            }
            //Consultar si existe un periodo igual en la base de datos
            if (await _context.Periodosmantenimientos.AnyAsync(x => x.PeriodoKilometraje == periodo))
            {
                return Json(new
                {
                    data = "",
                    type = "error",
                    mensaje = "Ocurrió un error al verificar los periodos, ya existe un registro con este valor"
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
                    mensaje = "Ocurrió un error al guardar los cambios en la base de datos"
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
                mensaje = "Se a agregado el periodo exitosamente"
            });
        }
        /// <summary>
        /// Método que se utiliza para agregar una nueva categoría 
        /// desde los formularios de agregar o editar vehículo.
        /// </summary>
        /// <param name="categoria">Nombre de la categoría.</param>
        /// <returns>Un json que contiene un mensaje y la categoría creada.</returns>
        public async Task<JsonResult> SelectAgregarCategoria(string categoria)
        {
            if (string.IsNullOrEmpty(categoria.Trim()))
            {
                return Json(new
                {
                    data = "",
                    type = "void",
                    mensaje = "Ocurrió un error al recibir los datos"
                });
            }
            var categorias = await _context.Categorias.ToListAsync();
            if (categorias.Any(x => x.Categoria1.Equals(categoria, StringComparison.OrdinalIgnoreCase)))
            {
                return Json(new
                {
                    data = "",
                    type = "error",
                    mensaje = "Ocurrió un error al verificar las categorías, ya existe un registro con este valor"
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
                    mensaje = "Ocurrió un error al guardar los cambios en la base de datos"
                });
            }

            return Json(new
            {
                data = c,
                type = "success",
                mensaje = "Se a agregado la categoría exitosamente"
            });
        }
        /// <summary>
        /// Método que se utiliza para obtener todos los periodos de mantención.
        /// </summary>
        /// <returns>Una lista de periodos de mantención para cargar en un Select HTML.</returns>
        private async Task<List<SelectListItem>> GetPeriodosMantencion()
        {
            var periodos =await _context.Periodosmantenimientos.OrderBy(p => p.PeriodoKilometraje)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = "Cada " + p.PeriodoKilometraje.ToString("N0") + " Km"
                }).ToListAsync();
            return periodos;
        }
        /// <summary>
        /// Método que se utiliza para obtener todas las categorías de los vehículos.
        /// </summary>
        /// <param name="vehiculos">Lista de vehículos filtrados</param>
        /// <returns>Una lista de categorías de vehículos para cargar en un Select HTML.</returns>
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
        /// <summary>
        /// Método que se utiliza para obtener todos los modelos de los vehículos.
        /// </summary>
        /// <param name="viewViehiculos">Lista de vehículos filtrados</param>
        /// <returns>Una lista de modelos de vehículos para cargar en un Select HTML.</returns>
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
        /// <summary>
        /// Método que se utiliza para obtener todas las marcas de los vehículos.
        /// </summary>
        /// <param name="viewViehiculos">Lista de vehículos filtrados</param>
        /// <returns>Una lista de marcas de vehículos para cargar en un Select HTML.</returns>
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
        /// <summary>
        /// Método que se utiliza para obtener todas las patentes de los vehículos.
        /// </summary>
        /// <param name="viewViehiculos">Lista de vehículos filtrados</param>
        /// <returns>Una lista de patentes de vehículos para cargar en un Select HTML.</returns>
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
        /// <summary>
        /// Método que se utiliza para obtener todas las categorías de los vehículos.
        /// </summary>
        /// <remarks>Se realiza un filtro de categorías las cuales el rol del usuario tiene disponible ver</remarks>
        /// <returns>Una lista de categorías de vehículos para cargar en un Select HTML.</returns>
        private async Task<IEnumerable> GetCategorias()
        {
            var categorias = await _context.Categorias.OrderBy(c => c.Categoria1).ToListAsync();

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
        /// <summary>
        /// Método que se utiliza para obtener todos los conductores.
        /// </summary>
        /// <returns>Una lista de conductores para cargar en un Select HTML.</returns>
        private async Task<List<SelectListItem>> GetConductores()
        {
            DateTime hoy = DateTime.Now;
            return await _context.Conductores.OrderBy(c => c.IdUsuarioNavigation.Nombre)
                .Where(c => c.FechaVencimiento >= hoy && !c.IdUsuarioNavigation.Eliminado)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.IdUsuarioNavigation.Nombre} {c.IdUsuarioNavigation.Apellido}"
                }).ToListAsync();
        }
        /// <summary>
        /// Método que se utiliza para obtener el conductor asignado a un vehículo.
        /// </summary>
        /// <returns>El identificador único del conductor en formato json.</returns>
        public async Task<JsonResult> GetConductorVehiculo(int id = 0)
        {
            int idconductor = 0;
            if (id == 0)
            {
                return Json(idconductor);
            }
            var vehiculo = await _context.Vehiculos.FirstOrDefaultAsync(v => v.Id == id);
            //Se verifica si existe conductor asignado en el vehículo
            if (vehiculo.IdConductor.HasValue)
            {
                idconductor = (int)vehiculo.IdConductor;
                return Json(idconductor);
            }
            return Json(idconductor);
        }
    }
}
