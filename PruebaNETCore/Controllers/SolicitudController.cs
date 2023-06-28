using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;
using ProyectoSolveCore.Models.ViewModelsFilter;
using System.Globalization;
using System.Security.Claims;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con las solicitudes de vehículos.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver, crear y modificar las solicitudes de los vehículos.
    /// </remarks>
    [Authorize]
    public class SolicitudController : Controller
    {
        /// <summary>
        /// Contexto de la base de datos
        /// </summary>
        private readonly ModelData _context;
        /// <summary>
        /// Objeto de la memoria cache
        /// </summary>
        private readonly IMemoryCache _cache; //Guarda datos en la memoria cache
        /// <summary>
        /// Formato largo para las fechas
        /// </summary>
        private const string FormatLong = "dd/MMMM/yy HH:mm";
        /// <summary>
        /// Formato corto para las fechas
        /// </summary>
        private const string FormatShort = "dd/MMMM/yyyy";
        //Constructor
        public SolicitudController(ModelData context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        /// <summary>
        /// Muestra todas las solicitudes realizadas.
        /// </summary>
        /// <returns>Una vista que muestra todas las solicitudes realizadas.</returns>
        [Autorizar(10)]
        [TypeFilter(typeof(VerificarSolicitudes))]
        public async Task<IActionResult> VisualizarSolicitudes()
        {
            ViewBag.notfilter = false;
            if (!await _context.Solicitudes.AnyAsync())
            {
                ViewBag.notfilter = true;
                return View(new List<Solicitud>());
            }
            try
            {
                var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation).Include(s => s.IdConductorNavigation.IdUsuarioNavigation)
                        .Include(s => s.IdVehiculoNavigation.IdCategoriaNavigation).Include(s => s.Aprobaciones)
                        .OrderByDescending(s => s.FechaSolicitado)
                        .ToListAsync();

                ViewBag.Motivo = string.Empty;
                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text");
                ViewBag.Destino = new SelectList(ObtenerDestinos(solicitudes), "Value", "Text");
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(solicitudes), "Value", "Text", null, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(solicitudes), "Value", "Text");
                ViewBag.Fechas = new SelectList(ObtenerOpcionesFiltrosFecha(), "Value", "Text");

                return View(solicitudes);
            }
            catch (Exception ex)
            {
                ViewBag.notfilter = true;
                return View(new List<Solicitud>());
            }
        }
        /// <summary>
        /// Visualiza todas las solicitudes según los filtros especificados.
        /// </summary>
        /// <param name="fs">Objeto que contiene los filtros de búsqueda de las solicitudes.</param>
        /// <returns>Una vista que muestra la vista con las solicitudes filtradas.</returns>
        [Autorizar(10)]
        [HttpPost]
        public async Task<IActionResult> VisualizarSolicitudes(vmFiltrosSolicitudes fs)
        {
            ViewBag.notfilter = false;
            try
            {
                var NewList = await FiltrarSolicitudes(fs);

                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text", fs.Estado);
                ViewBag.Destino = new SelectList(ObtenerDestinos(NewList), "Value", "Text", fs.Destino);
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(NewList), "Value", "Text", fs.Vehiculo, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(NewList), "Value", "Text", fs.IdSolicitado);
                ViewBag.Motivo = fs.Motivo;

                return View(NewList);
            }
            catch (Exception ex)
            {
                ViewBag.notfilter = true;
                var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation).Include(s => s.IdVehiculoNavigation)
                        .ToListAsync();
                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text", fs.Estado);
                ViewBag.Destino = new SelectList(ObtenerDestinos(solicitudes), "Value", "Text", fs.Destino);
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(solicitudes), "Value", "Text", fs.Vehiculo, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(solicitudes), "Value", "Text", fs.IdSolicitado);
                ViewBag.Motivo = fs.Motivo;
                return View(solicitudes);
            }
        }

        //Este método funciona para gestionar las solicitudes que están aprobadas y listas para agregar a la bitácora
        /// <summary>
        /// Gestiona las solicitudes aprobadas para agregar a la bitácora.
        /// </summary>
        /// <returns>Una vista que redirige a la acción para agregar entradas en la bitácora.</returns>
        public async Task<IActionResult> HubSolicitudes()
        {
            EliminarMemoriaCache();
            try
            {
                //Se configura la cache para que dentro de 10 minutos se borre automáticamente los datos guardados
                MemoryCacheEntryOptions opciones = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                int id = 0;
                //Obtengo una lista de solicitudes que estén aprobadas y que ya sea han realizado
                var slist = await _context.Solicitudes.Include(s => s.IdConductorNavigation.IdUsuarioNavigation).Include(s => s.IdVehiculoNavigation)
                    .OrderBy(s => s.FechaSolicitado)
                    .Where(s => s.Estado == 1 && s.FechaLlegada >= DateTime.Now)
                    .ToListAsync();
                if (slist.Count == 0)
                {//Si no existen ninguna solicitud, devolver a la vista "MisSolicitudes"
                    return RedirectToAction("MisSolicitudes", "Solicitud");
                }
                //Se verifican los roles esenciales para este método,
                //si es jefe o mantenedor de la bitácora
                if (VerificarRolAdmin() || VerificarRolBitacora())
                {//Con esto recuperamos todas las solicitudes listas para agregar a la bitácora
                    //Se crea una variable en la memoria cache con la cantidad de solicitudes
                    _cache.Set("NumeroSolicitudes", slist.Count, opciones); 
                    id = slist.FirstOrDefault().Id;
                    return RedirectToAction("AgregarEntradasBitacora", "Bitacora", new
                    {
                        id
                    });
                }
                //Obtengo el id del usuario en sesión
                int idUsuario = int.Parse(User.FindFirst("Id").Value);
                //Recupero las solicitudes del usuario
                var slist_user = slist.Where(s => s.IdSolicitante == idUsuario).ToList();
                if (slist_user.Count == 0)
                {//Si no existen solicitudes, devolver a la vista "MisSolicitudes"
                    return RedirectToAction("MisSolicitudes", "Solicitud");
                }
                //Se crea una variable en la memoria cache con la cantidad de solicitudes
                _cache.Set("NumeroSolicitudes", slist_user.Count, opciones);
                id = slist_user.FirstOrDefault().Id;
                return RedirectToAction("AgregarEntradasBitacora", "Bitacora", new
                {
                    id
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("MisSolicitudes", "Solicitud");
            }
        }
        /// <summary>
        /// Muestra las aprobaciones realizadas por el usuario actual.
        /// </summary>
        /// <returns>Una vista que muestra las aprobaciones realizadas.</returns>
        public async Task<IActionResult> MisAprobaciones()
        {
            try
            {
                int idUser = int.Parse(User.FindFirst("Id").Value);
                var aprobaciones = await _context.Aprobaciones.Include(a => a.IdJefeNavigation)
                    .Where(a => a.IdJefe == idUser)
                    .OrderByDescending(a => a.Fecha)
                    .ToListAsync();

                if (!aprobaciones.Any())
                {
                    return View(new List<vmSolicitud>());
                }

                return View(aprobaciones);
            }
            catch (Exception ex)
            {
                return View(new List<Aprobacion>());
            }
        }
        /// <summary>
        /// Muestra las solicitudes del usuario actual.
        /// </summary>
        /// <returns>Una vista que muestra las solicitudes del usuario.</returns>
        [TypeFilter(typeof(VerificarSolicitudes))]
        public async Task<IActionResult> MisSolicitudes()
        {
            ViewBag.notfilter = false;
            try
            {
                int Id = int.Parse(User.FindFirst("Id").Value);
                var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation)
                    .Include(s => s.IdConductorNavigation.IdUsuarioNavigation).Include(s => s.Aprobaciones)
                    .Include(s => s.IdVehiculoNavigation.IdCategoriaNavigation)
                    .OrderByDescending(s => s.FechaSolicitado)
                    .ToListAsync();

                if (!solicitudes.Any(s => s.IdSolicitante == Id))
                {
                    ViewBag.notfilter = true;
                    return View(new List<vmSolicitud>());
                }

                var listSolicitudes = solicitudes.Where(s => s.IdSolicitante == Id)
                        .Select(s => new vmSolicitud()
                        {
                            id = s.Id,
                            FechaSolicitado = s.FechaSolicitado,
                            FechaLlegada = s.FechaLlegada,
                            FechaSalida = s.FechaSalida,
                            Estado = s.Estado,
                            vehiculo = s.IdVehiculoNavigation.Patente + " - " + s.IdVehiculoNavigation.Marca + " " + s.IdVehiculoNavigation.Modelo,
                            nombreConductor = s.IdConductorNavigation != null 
                                ? s.IdConductorNavigation.IdUsuarioNavigation.Nombre + " " + s.IdConductorNavigation.IdUsuarioNavigation.Apellido 
                                : "Sin conductor asignado",
                            CantidadAprobacion = s.Aprobaciones.Count
                        }).ToList();

                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text");
                ViewBag.Destino = new SelectList(ObtenerDestinos(solicitudes), "Value", "Text");
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(solicitudes), "Value", "Text", null, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(solicitudes), "Value", "Text");
                ViewBag.Motivo = string.Empty;

                return View(listSolicitudes);
            }
            catch (Exception ex)
            {
                ViewBag.notfilter = true;
                return View(new List<vmSolicitud>());
            }
        }
        /// <summary>
        /// Realiza la acción de filtro y muestra las solicitudes del usuario actual.
        /// </summary>
        /// <param name="fs">Los filtros de búsqueda de solicitudes.</param>
        /// <returns>Una vista que muestra las solicitudes del usuario.</returns>
        [HttpPost]
        public async Task<IActionResult> MisSolicitudes(vmFiltrosSolicitudes fs)
        {
            ViewBag.notfilter = false;
            int id = int.Parse(User.FindFirst("Id").Value);
            try
            {
                var NewList = await FiltrarSolicitudes(fs);

                var misolicitudes = NewList.Where(s => s.IdSolicitante == id).Select(s => new vmSolicitud()
                {
                    id = s.Id,
                    FechaSolicitado = s.FechaSolicitado,
                    FechaLlegada = s.FechaLlegada,
                    FechaSalida = s.FechaSalida,
                    Estado = s.Estado,
                    vehiculo = s.IdVehiculoNavigation.Patente + " - " + s.IdVehiculoNavigation.Marca +
                            " " + s.IdVehiculoNavigation.Modelo,
                    nombreConductor = s.IdConductorNavigation.IdUsuarioNavigation.Nombre +
                            " " + s.IdConductorNavigation.IdUsuarioNavigation.Apellido,
                    CantidadAprobacion = s.Aprobaciones.Count
                }).ToList();

                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text", fs.Estado);
                ViewBag.Destino = new SelectList(ObtenerDestinos(NewList), "Value", "Text", fs.Destino);
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(NewList), "Value", "Text", fs.Vehiculo, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(NewList), "Value", "Text", fs.IdSolicitado);
                ViewBag.Motivo = fs.Motivo;

                return View(misolicitudes);
            }
            catch (Exception)
            {
                ViewBag.notfilter = true;
                return View(new List<vmSolicitud>());
            }
        }
        /// <summary>
        /// Obtiene más información sobre una solicitud y muestra una vista parcial para un ventana emergente o modal con los detalles.
        /// </summary>
        /// <param name="id">El identificador único de la solicitud.</param>
        /// <param name="aprobacion">Indica si se trata de una aprobación o no (valor predeterminado: true).</param>
        /// <returns>Una vista parcial que muestra los detalles de la solicitud.</returns>
        public async Task<PartialViewResult> MasInformacionSolicitud(int? id, bool aprobacion = true)
        {
            try
            {
                string mensaje = string.Empty;
                if (!id.HasValue)
                {
                    mensaje = "Hubo un error al recibir los datos, recargue la página o inténtelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }

                var solicitud = await _context.Solicitudes
                    .Include(s => s.IdVehiculoNavigation).Include(s => s.IdSolicitanteNavigation)
                    .Include(s => s.IdConductorNavigation.IdUsuarioNavigation).Include(s => s.Aprobaciones)
                    .Where(s => s.Id == id).FirstOrDefaultAsync();

                if (solicitud == null)
                {
                    mensaje = "Hubo un error al cargar los datos, recargue la página o inténtelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }
                vmMasInformacionSolicitud infosolicitud = new()
                {
                    solicitud = solicitud,
                    aprobacion = aprobacion
                };
                return PartialView("_MasInformacionSolicitud", infosolicitud);
            }
            catch (Exception)
            {
                string mensaje = "Ocurrió un error inesperado, consulte con administrador del sistema o inténtelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }
        /// <summary>
        /// Muestra la vista para solicitar un vehículo.
        /// </summary>
        /// <returns>Una acción IActionResult que muestra la vista de solicitud de vehículo.</returns>
        [Autorizar(permisoId:2)]
        public async Task<IActionResult> SolicitarVehiculo()
        {
            try
            {
                //Consigo el id del usuario en sesión
                int idUser = int.Parse(User.FindFirst("Id").Value);
                var departamentos = await _context.Departamentos.ToListAsync();
                var usuarios = await _context.Usuarios.Where(u => !u.Eliminado && u.Id != 1)
                .Select(u => new vmUsuarioDepartamento()
                {
                    Id = u.Id,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    IdDepartamento = u.IdDepartamento
                })
                .ToListAsync();
                if (idUser != 2)
                {
                    usuarios = usuarios.Where(u => u.Id != 2).ToList();
                }

                var usuario = usuarios.FirstOrDefault(u => u.Id == idUser);
                ViewBag.IdSolicitante = new SelectList(usuarios, "Id", "NombreCompleto", idUser);
                ViewBag.Departamento = await _context.Departamentos.Where(d => d.Id == usuario.IdDepartamento).FirstOrDefaultAsync();
                return View(new Solicitud()
                {
                    IdSolicitante = idUser
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("MisSolicitudes");
            }
        }
        /// <summary>
        /// Crea una nueva solicitud de vehículo.
        /// </summary>
        /// <param name="solicitud">La solicitud de vehículo proporcionada.</param>
        /// <returns>Una tarea que representa la operación sincrónica y devuelve un IActionResult.</returns>
        [Autorizar(permisoId:2)]
        [HttpPost]
        public async Task<IActionResult> SolicitarVehiculo(Solicitud solicitud)
        {
            try
            {
                if (solicitud == null)
                {
                    return View(new Solicitud());
                }
                
                var vehiculo = await _context.Vehiculos.Where(v => v.Id == solicitud.IdVehiculo).FirstOrDefaultAsync();
                // Convertir el JSON a una lista de objetos
                List<PasajerosAux> valueList = JsonConvert.DeserializeObject<List<PasajerosAux>>(solicitud.Pasajeros);
                //Unir todo a cadena de texto
                solicitud.Pasajeros = string.Join(", ", valueList.ConvertAll(x => x.value));
                solicitud.FechaSolicitado = DateTime.Now;
                //Verificar si existe un conductor asignado a un vehículo
                if (vehiculo.IdConductor.HasValue)
                {//Si existe, asignarlo a la solicitud
                    solicitud.IdConductor = vehiculo.IdConductor;
                }

                await _context.Solicitudes.AddAsync(solicitud);

                int n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    return View(solicitud);
                }
                return RedirectToAction("MisSolicitudes");
            }
            catch (Exception ex)
            {
                return View(new Solicitud());
            }
        }
        /// <summary>
        /// Obtiene la lista de vehículos disponibles para un rango de fechas.
        /// </summary>
        /// <param name="datos">Las fechas seleccionadas por los usuarios</param>
        /// <returns>Un objeto JsonResult con la lista de vehículos disponibles.</returns>
        [HttpPost]
        public async Task<JsonResult> GetVehiculos([FromBody] vmFechaSalidaLlegada datos)
        {
            try
            {
                //Convierto las fechas
                if (!DateTime.TryParse(datos.fecha_salida, out DateTime fecha_salida)
                || !DateTime.TryParse(datos.fecha_llegada, out DateTime fecha_llegada))
                {//Error al convertir las fechas
                    return Json(new
                    {
                        data = new List<SelectListItem>(),
                        mensaje = "Al parecer las fechas seleccionadas son incorrectas o algunos campos están vacíos, contacte con el administrador del sitio o intente más tarde",
                        type = "danger"
                    });
                }
                //Recupero las solicitudes que están dentro del rango de fecha recibido
                var solicitudes = await _context.Solicitudes
                    .Where(s => s.Estado < 2 && (s.FechaSalida <= fecha_salida && s.FechaLlegada >= fecha_salida)
                                             || (s.FechaLlegada >= fecha_llegada && s.FechaSalida <= fecha_llegada))
                    .ToListAsync();

                if (solicitudes.Count == 0)
                {//Si no existen solicitudes
                    return Json(new
                    {
                        //Devuelvo todos los vehículos que estén habilitados
                        data = await _context.Vehiculos
                            .Where(v => v.Estado)
                            .Select(v => new SelectListItem
                            {
                                Value = v.Id.ToString(),
                                Text = $"{v.Patente} - {v.Marca} {v.Modelo}"
                            })
                            .ToListAsync(),
                        type = "success"
                    });
                }
                //Recupero todos los vehículos ocupados
                var vehiculosIdsOcupados = solicitudes.Select(s => s.IdVehiculo).ToList();
                //Recupero los vehículos disponibles
                var vehiculosDisponibles = await _context.Vehiculos
                    .Where(v => v.Estado && !vehiculosIdsOcupados.Contains(v.Id))
                    .Select(v => new SelectListItem
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}"
                    })
                    .ToListAsync();

                return Json(new
                {
                    data = vehiculosDisponibles,
                    type = "success"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    data = new List<SelectListItem>(),
                    mensaje = "Ocurrió un error inesperado, contacte con administrador del sitio o intente mas tarde",
                    type = "danger"
                });
            }
        }
        //SOLICITUDES PENDIENTES
        /// <summary>
        /// Recupera las solicitudes pendientes para mostrarlas en la vista
        /// </summary>
        /// <returns>La vista con las solicitudes pendientes.</returns>
        [Autorizar(permisoId:15)]
        [TypeFilter(typeof(VerificarSolicitudes))]
        public async Task<IActionResult> SolicitudesPendientes()
        {
            //ViewBag que sirve para saber si se debe mostrar el filtro en la vista
            ViewBag.notfilter = false;
            try
            {
                int id = int.Parse(User.FindFirst("Id").Value);
                var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation).Include(s => s.IdConductorNavigation.IdUsuarioNavigation)
                        .Include(s => s.IdVehiculoNavigation.IdCategoriaNavigation).Include(s => s.Aprobaciones)
                        .Where(s => s.Estado == 0)
                        .OrderByDescending(s => s.FechaSolicitado)
                        .ToListAsync();

                if (!solicitudes.Any())
                {
                    ViewBag.notfilter = true;
                    return View(new List<Solicitud>());
                }

                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text");
                ViewBag.Destino = new SelectList(ObtenerDestinos(solicitudes), "Value", "Text");
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(solicitudes), "Value", "Text", null, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(solicitudes), "Value", "Text");
                ViewBag.Motivo = string.Empty;

                return View(solicitudes);
            }
            catch (Exception ex)
            {
                ViewBag.notfilter = true;
                return View(new List<Solicitud>());
            }
        }
        /// <summary>
        /// Acción de la vista Solicitudes pendientes para poder recargar la tabla con las solicitudes pendientes filtradas
        /// </summary>
        /// <param name="fs">Parámetros del filtro</param>
        /// <returns>Una vista con las solicitudes pendientes filtradas.</returns>
        [HttpPost]
        public async Task<IActionResult> SolicitudesPendientes(vmFiltrosSolicitudes fs)
        {
            ViewBag.notfilter = false;
            try
            {
                var solicitudes = await FiltrarSolicitudes(fs);
                solicitudes = solicitudes.Where(s => s.Estado == 0).ToList();

                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text", fs.Estado);
                ViewBag.Destino = new SelectList(ObtenerDestinos(solicitudes), "Value", "Text", fs.Destino);
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(solicitudes), "Value", "Text", fs.Vehiculo, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(solicitudes), "Value", "Text", fs.IdSolicitado);
                ViewBag.Motivo = fs.Motivo;

                return View(solicitudes);
            }
            catch (Exception ex)
            {
                var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation).Include(s => s.IdVehiculoNavigation)
                    .Where(s => s.Estado == 0)
                    .ToListAsync();

                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text");
                ViewBag.Destino = new SelectList(ObtenerDestinos(solicitudes), "Value", "Text");
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(solicitudes), "Value", "Text", null, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(solicitudes), "Value", "Text");
                ViewBag.Motivo = fs.Motivo;

                return View(solicitudes);
            }
        }
        /// <summary>
        /// Asigna un conductor a una solicitud.
        /// </summary>
        /// <param name="id">El ID de la solicitud.</param>
        /// <returns>Un objeto PartialViewResult con la vista parcial para asignar un conductor.</returns>
        [Autorizar(permisoId:3)]
        public async Task<PartialViewResult> AsignarConductor(int? id)
        {
            try
            {
                // Obtener la solicitud correspondiente con sus relaciones incluidas
                var solicitud = await _context.Solicitudes.Include(s => s.IdVehiculoNavigation).Include(s => s.IdConductorNavigation.IdUsuarioNavigation).Include(s => s.IdSolicitanteNavigation)
                    .Where(s => s.Id == id).FirstOrDefaultAsync();
                Usuario usuario = solicitud.IdSolicitanteNavigation;
                List<vmConductoresList> conductores = new();
                int ConductorAsignado = solicitud.IdVehiculoNavigation.IdConductor??0;
                //Verifico si a la solicitud se le asigno un conductor
                if (!solicitud.IdConductor.HasValue)
                {//Si no tiene valor es porque no esta asignado y consigo todos los conductores disponibles
                    conductores = await GetConductoresDisponibles(solicitud);
                }
                else
                {//Consigo todos los conductores
                    conductores = await _context.Conductores.Include(c => c.IdUsuarioNavigation)
                    .Where(c => c.Id != id && !c.IdUsuarioNavigation.Eliminado && c.Estado).Select(c => new vmConductoresList()
                    {
                        Id = c.Id,
                        Nombre = c.IdUsuarioNavigation.Nombre + " " + c.IdUsuarioNavigation.Apellido,
                        rut = c.IdUsuarioNavigation.Rut
                    }).ToListAsync();
                }
                
                ViewBag.NombreSolicitante = usuario.Nombre + " " + usuario.Apellido;
                //Al pasarle la lista de conductores al selectlist hago una lógica la cual es la siguiente:
                //Si el id del conductor asignado es diferente a 0 se le asigna el valor del atributo
                ViewBag.IdConductor = new SelectList(conductores, "Id", "Nombre", (ConductorAsignado != 0) ? ConductorAsignado : null);
                return PartialView("_AsignarConductor", solicitud);
            }
            catch (Exception)
            {
                string mensaje = "Ocurrió un error inesperado, consulte con administrador del sistema o inténtelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }
        /// <summary>
        /// Aprueba una solicitud.
        /// </summary>
        /// <param name="ids">Los IDs del conductor y la solicitud.</param>
        /// <returns>Un objeto JsonResult con el resultado de la operación.</returns>
        [Autorizar(permisoId:3)]
        [HttpPost]
        public async Task<JsonResult> AprobarSolicitud([FromBody] vmIdConductorSolicitud ids)
        {
            bool exist = VerificarRolJefe();
            if (!exist)
            {
                return Json(new
                {
                    mensaje = "Hubo un error al verificar su rol. Al parecer no tiene los permisos suficientes para aprobar solicitudes.",
                    type = "danger"
                });
            }
            try
            {
                int id = int.Parse(User.FindFirst("Id").Value);
                if (ids.IdConductor == null)
                {
                    return Json(new
                    {
                        mensaje = "Hubo un error al enviar los datos. Asegúrese que haya seleccionado un conductor.",
                        type = "danger"
                    });
                }
                // Iniciar el proceso de transacción a la base de datos
                using var transaction = await _context.Database.BeginTransactionAsync();
                //Obtiene la solicitud con su identificador único recibido
                var solicitud = await _context.Solicitudes.FindAsync(ids.IdSolicitud);
                if (solicitud == null)
                {
                    return Json(new
                    {
                        mensaje = "Hubo un error al obtener los datos, inténtelo nuevamente",
                        type = "danger"
                    });
                }

                //Modificamos el id del conductor en el parámetro IdConductor de la solicitud
                solicitud.IdConductor = ids.IdConductor;

                var aprobacion = new Aprobacion()
                {
                    Fecha = DateTime.Now,
                    Estado = true,
                    Motivo = null,
                    IdJefe = id,
                    IdSolicitud = ids.IdSolicitud
                };

                await _context.Aprobaciones.AddAsync(aprobacion);
                int n = await _context.SaveChangesAsync();
                //Se cambia el estado a la solicitud a 1 = aprobada
                solicitud.Estado = 1;
                n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    return Json(new
                    {
                        mensaje = "Hubo un error al guardar los datos, inténtelo nuevamente",
                        type = "danger"
                    });
                }
                // Commit de la transacción a la base de datos
                await transaction.CommitAsync();
                return Json(new
                {
                    mensaje = "Se a guardado correctamente su aprobación",
                    type = "success"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    mensaje = "Ocurrió un error inesperado, consulte con administrador del sistema o inténtelo nuevamente",
                    type = "danger"
                });
            }
        }

        /// <summary>
        /// Carga una vista para el modal o ventana emergente para la confirmación del rechazo de la solicitud.
        /// </summary>
        /// <param name="id">Identificador único de la solicitud</param>
        /// <returns>Una vista parcial para cargarlo en una ventana emergente</returns>
        [Autorizar(permisoId:3)]
        public async Task<PartialViewResult> ConfirmacionRechazar(int id = 0)
        {
            try
            {
                string mensaje = string.Empty;
                if (id == 0)
                {
                    mensaje = "Hubo un error al recibir los datos, recargue la página o inténtelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }
                //Se obtiene la solicitud filtrando con el identificador unico recibido
                var solicitud = await _context.Solicitudes.Include(s => s.IdVehiculoNavigation).Include(s => s.IdSolicitanteNavigation)
                    .Where(s => s.Id == id).FirstOrDefaultAsync();

                if (solicitud == null)
                {
                    mensaje = "Hubo un error al obtener los datos, recargue la página o inténtelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }
                //Se obtiene el usuario solicitante
                var usuario = solicitud.IdSolicitanteNavigation;
                ViewBag.NombreSolicitante = usuario.Nombre + " " + usuario.Apellido;

                return PartialView("_ConfirmarRechazar", solicitud);
            }
            catch (Exception)
            {
                string mensaje = "Ocurrió un error inesperado, consulte con administrador del sistema o inténtelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }
        /// <summary>
        /// Rechaza una solicitud.
        /// </summary>
        /// <param name="datos">Los datos de la solicitud a rechazar.</param>
        /// <returns>Un objeto JsonResult con el resultado de la operación.</returns>
        [Autorizar(permisoId:3)]
        [HttpPost]
        public async Task<JsonResult> RechazarSolicitud([FromBody] vmRechazarSolicitud datos)
        {
            bool exist = VerificarRolJefe();
            if (!exist)
            {
                return Json(new
                {
                    mensaje = "Hubo un error al verificar su rol. Al parecer no tiene los permisos suficientes para aprobar solicitudes.",
                    type = "danger"
                });
            }
            try
            {
                int id = int.Parse(User.FindFirst("Id").Value);

                //Iniciar el proceso de transacción a la base de datos
                using var transaction = await _context.Database.BeginTransactionAsync();
                var solicitud = await _context.Solicitudes.FindAsync(datos.id_solicitud);
                if (solicitud == null)
                {
                    return Json(new
                    {
                        mensaje = "Hubo un error al obtener los datos, inténtelo nuevamente",
                        type = "danger"
                    });
                }

                var aprobacion = new Aprobacion()
                {
                    Fecha = DateTime.Now,
                    Estado = false,
                    Motivo = datos.motivo,
                    IdJefe = id,
                    IdSolicitud = datos.id_solicitud
                };

                await _context.Aprobaciones.AddAsync(aprobacion);
                //Cambiar el estado de la solicitud a 2 = Rechazada
                solicitud.Estado = 2;
                int n = await _context.SaveChangesAsync();
                
                //Commit de la transacción a la base de datos
                await transaction.CommitAsync();
                return Json(new
                {
                    mensaje = "Se a rechazado la solicitud satisfactoriamente",
                    type = "success"
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    mensaje = "Ocurrió un error inesperado, consulte con administrador del sistema o inténtelo nuevamente",
                    type = "danger"
                });
            }
        }
        //METODO EL CUAL OBTIENE TODAS LAS SOLICITUDES PARA MOSTRARLAS EN LA LIBRERIA CALENDAR
        /// <summary>
        /// Obtiene todas las solicitudes para mostrarlas en la librería de calendario.
        /// </summary>
        /// <returns>Un json que contiene las solicitudes.</returns>
        public async Task<JsonResult> GetSolicitudes()
        {
            //Configuro las fechas en un formato chileno
            CultureInfo.CurrentCulture = new CultureInfo("es-CL");
            try
            {
                int year = DateTime.Now.Year;
                DateTime primerDiaDelAnio = new(year, 1, 1);//Primer día del año
                DateTime ultimoDiaDelAnio = new(year, 12, 31);//Ultimo día del año

                var solicitudes = await _context.Solicitudes.Include(s => s.IdVehiculoNavigation).Include(s => s.IdConductorNavigation.IdUsuarioNavigation)
                    .Where(s => s.Estado == 1 || s.Estado == 3).Where(s => s.FechaSalida >= primerDiaDelAnio && s.FechaSalida <= ultimoDiaDelAnio)
                    .Select(s => new vmSolicitud()
                    {
                        id = s.Id,
                        fechaLongSalidaStr = s.FechaSalida.ToString(FormatLong),
                        fechaSalidaStr = s.FechaSalida.ToString(FormatShort),
                        fechaLongLlegadaStr = s.FechaLlegada.ToString(FormatLong),
                        fechaLlegadaStr = s.FechaLlegada.ToString(FormatShort),
                        nombreConductor = s.IdConductorNavigation.IdUsuarioNavigation.Nombre + " " + s.IdConductorNavigation.IdUsuarioNavigation.Apellido,
                        vehiculo = s.IdVehiculoNavigation.Patente + " - " + s.IdVehiculoNavigation.Marca + " " + s.IdVehiculoNavigation.Modelo,
                        motivo = s.Motivo
                    }).ToListAsync();
                return Json(solicitudes);
            }
            catch (Exception)
            {
                return Json(new
                {
                    mensaje = "Ocurrió un error inesperado, contacte con administrador del sitio o intente mas tarde",
                    type = "danger"
                });
            }
        }
        /// <summary>
        /// Obtiene el departamento asociado a un usuario específico.
        /// </summary>
        /// <param name="id">El identificador del usuario. Valor predeterminado: 0.</param>
        /// <returns>Un objeto json con información sobre el departamento.</returns>
        public async Task<JsonResult> SelectGetDepartamento(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return Json(new
                    {
                        Mensaje = "Ocurrió un error al recibir los datos",
                        Departamento = string.Empty,
                    });
                }
                var usuario = await _context.Usuarios.Include(u => u.IdDepartamentoNavigation).Where(u => u.Id == id)
                    .FirstOrDefaultAsync();
                if (usuario == null)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrió un error al obtener los datos",
                        Departamento = string.Empty
                    });
                }
                string departamento = usuario.IdDepartamentoNavigation.Departamento1;
                return Json(new
                {
                    Mensaje = "Departamento obtenido exitosamente",
                    Departamento = departamento
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Mensaje = "Ocurrió un error inesperado. Avise al administrador del sistema o inténtelo nuevamente",
                    Departamento = string.Empty
                });
            }
        }
        /// <summary>
        /// Obtiene una lista de conductores disponibles para una solicitud específica.
        /// </summary>
        /// <param name="solicitud">La solicitud para la cual se buscan los conductores disponibles.</param>
        /// <returns>Una lista de objetos vmConductoresList que representan los conductores disponibles.</returns>
        private async Task<List<vmConductoresList>> GetConductoresDisponibles(Solicitud solicitud)
        {
            List<int?> ConductoresOcupadosId = new();
            List<vmConductoresList> conductoresDisponibles = new();

            // Obtener las solicitudes que se superponen en el tiempo con la solicitud actual
            var SolicitudesInSolicitud = await _context.Solicitudes
                .Where(s => (s.FechaSalida <= solicitud.FechaSalida && s.FechaLlegada >= solicitud.FechaLlegada)
                        || (s.FechaLlegada >= solicitud.FechaLlegada && s.FechaSalida <= solicitud.FechaLlegada))
                .Where(s => s.Estado < 2 && s.IdConductor.HasValue).ToListAsync();

            if (SolicitudesInSolicitud.Count == 0)
            {// Si no hay solicitudes superpuestas, obtener todos los conductores disponibles
                conductoresDisponibles = await _context.Conductores.Include(c => c.IdUsuarioNavigation)
                    .Where(c => !c.IdUsuarioNavigation.Eliminado && c.Estado)
                    .Select(c => new vmConductoresList()
                    {
                        Id = c.Id,
                        Nombre = c.IdUsuarioNavigation.Nombre + " " + c.IdUsuarioNavigation.Apellido,
                        rut = c.IdUsuarioNavigation.Rut
                    }).ToListAsync();

                return conductoresDisponibles;
            }
            // Obtener los IDs de los conductores ocupados en las solicitudes superpuestas
            ConductoresOcupadosId = SolicitudesInSolicitud.Select(s => s.IdConductor).ToList();
            foreach (var id in ConductoresOcupadosId)
            {
                // Obtener los conductores disponibles excluyendo los conductores ocupados y eliminados
                conductoresDisponibles = await _context.Conductores.Include(c => c.IdUsuarioNavigation)
                    .Where(c => c.Id != id && !c.IdUsuarioNavigation.Eliminado && c.Estado).Select(c => new vmConductoresList()
                    {
                        Id = c.Id,
                        Nombre = c.IdUsuarioNavigation.Nombre + " " + c.IdUsuarioNavigation.Apellido,
                        rut = c.IdUsuarioNavigation.Rut
                    }).ToListAsync();
            }
            return conductoresDisponibles;
        }
        /// <summary>
        /// Obtiene una lista de estados.
        /// </summary>
        /// <returns>Una lista de objetos SelectListItem con los estados.</returns>
        private static List<SelectListItem> ObtenerEstados()
        {
            Dictionary<int, string> EstadosDicc = new()
            {
                { 0, "Pendiente" },
                { 1, "Aprobada" },
                { 2, "Rechazada" },
                { 3, "Finalizada" },
            };

            List<SelectListItem> list = EstadosDicc
            .Select(e => new SelectListItem
            {
                Value = e.Key.ToString(),
                Text = e.Value
            })
            .ToList();
            return list;
        }
        /// <summary>
        /// Obtiene una lista de destinos a partir de una lista de solicitudes.
        /// </summary>
        /// <param name="solicitudes">La lista de solicitudes.</param>
        /// <returns>Una lista de objetos SelectListItem con los destinos encontrados en las solicitudes.</returns>
        private static List<SelectListItem> ObtenerDestinos(List<Solicitud> solicitudes)
        {
            //Se recuperan los destinos en una lista, sin que se repitan, sin importar las palabras mayusculas y minusculas
            List<SelectListItem> list = new();
            var destinos = solicitudes.Select(s => s.Destino)
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var d in destinos)
            {
                // Verificar si el destino existe en alguna solicitud
                if (solicitudes.Any(s => s.Destino.Equals(d)))
                {
                    list.Add(new SelectListItem
                    {
                        Value = d,
                        Text = d
                    });
                }
            }
            return list;
        }
        /// <summary>
        /// Obtiene una lista de vehículos a partir de una lista de solicitudes.
        /// </summary>
        /// <param name="solicitudes">La lista de solicitudes.</param>
        /// <returns>Una lista de objetos SelectListWithGroups con los vehículos encontrados en las solicitudes.</returns>
        private static List<SelectListWithGroups> ObtenerVehiculo(List<Solicitud> solicitudes)
        {
            List<SelectListWithGroups> list = new();
            //Se recuperan los vehículos que no han sido eliminados en una lista, sin que se repitan
            var vehiculos = solicitudes.Select(s => s.IdVehiculoNavigation)
                .Distinct().Where(v => !v.Eliminado).ToList();
            foreach (var v in vehiculos)
            {
                // Verificar si el vehículo existe en alguna solicitud
                if (solicitudes.Any(s => s.IdVehiculo == v.Id))
                {
                    list.Add(new SelectListWithGroups()
                    {
                        Value = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    });
                }
            }
            return list;
        }
        /// <summary>
        /// Obtiene una lista de usuarios a partir de una lista de solicitudes.
        /// </summary>
        /// <param name="solicitudes">La lista de solicitudes.</param>
        /// <returns>Una lista de objetos SelectListItem con los usuarios encontrados en las solicitudes.</returns>
        private static List<SelectListItem> ObtenerUsuarios(List<Solicitud> solicitudes)
        {
            List<SelectListItem> list = new();
            var usuarios = solicitudes.Select(s => s.IdSolicitanteNavigation).Distinct().ToList();

            foreach (var u in usuarios)
            {
                // Verificar si el usuario existe en alguna solicitud
                var exist = solicitudes.Any(s => s.IdSolicitante == u.Id);
                if (exist)
                {
                    list.Add(new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = $"{u.Nombre} {u.Apellido}",
                    });
                }
            }

            return list;
        }
        /// <summary>
        /// Filtra la lista de solicitudes según los criterios especificados en los filtros.
        /// </summary>
        /// <param name="fs">Los filtros de solicitud.</param>
        /// <returns>Una lista de solicitudes filtradas.</returns>
        private async Task<List<Solicitud>> FiltrarSolicitudes(vmFiltrosSolicitudes fs)
        {
            var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation).Include(s => s.IdConductorNavigation.IdUsuarioNavigation)
                        .Include(s => s.IdVehiculoNavigation.IdCategoriaNavigation).Include(s => s.Aprobaciones)
                        .OrderByDescending(s => s.FechaSolicitado)
                        .ToListAsync();

            if (fs == null
                || (fs.Estado == -1 && fs.OpcionFecha == -1 && string.IsNullOrEmpty(fs.Destino) && string.IsNullOrEmpty(fs.Vehiculo) && string.IsNullOrEmpty(fs.Motivo)))
            {//Todos los parámetros son los valores por defecto, devolver la lista sin filtrar
                return solicitudes;
            }
            if (fs.Estado > -1)
            {
                solicitudes = solicitudes.Where(s => s.Estado == fs.Estado).ToList();
            }
            if (fs.Estado > -1)
            {//FILTRAR POR LA FECHA
                solicitudes = solicitudes.Where(s => s.Estado == fs.Estado).ToList();
            }
            if (!string.IsNullOrEmpty(fs.Motivo))
            {
                solicitudes = solicitudes.Where(s => s.Motivo.Contains(fs.Motivo, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(fs.Destino))
            {
                solicitudes = solicitudes.Where(s => s.Destino.Equals(fs.Destino)).ToList();
            }
            if (!string.IsNullOrEmpty(fs.Vehiculo))
            {
                solicitudes = solicitudes.Where(s =>
                    string.Equals($"{s.IdVehiculoNavigation.Patente} - {s.IdVehiculoNavigation.Marca} {s.IdVehiculoNavigation.Modelo}",
                        fs.Vehiculo)).ToList();
            }
            return solicitudes;
        }
        /// <summary>
        /// Se crea una lista de tipos de filtros de fechas para cargarla en una etiqueta Select
        /// </summary>
        private static List<SelectListItem> ObtenerOpcionesFiltrosFecha()
        {
            List<SelectListItem> sli = new()
            {
                new SelectListItem(){ Value = "1", Text = "Filtrar solicitudes creadas hoy" },
                new SelectListItem(){ Value = "2", Text = "Filtrar solicitudes creadas en las últimas 24 horas" },
                new SelectListItem(){ Value = "3", Text = "Filtrar solicitudes creadas esta semana" },
                new SelectListItem(){ Value = "4", Text = "Filtrar solicitudes creadas en los últimos 7 días" },
                new SelectListItem(){ Value = "5", Text = "Filtrar solicitudes creadas este mes" },
                new SelectListItem(){ Value = "6", Text = "Filtrar solicitudes creadas en el último mes"},
                new SelectListItem(){ Value = "7", Text = "Filtrar solicitudes creadas este año" },
                new SelectListItem(){ Value = "8", Text = "Filtrar solicitudes creadas en los últimos 12 meses" }
            };
            return sli;
        }
        /// <summary>
        /// Elimina los elementos de la memoria cache.
        /// </summary>
        private void EliminarMemoriaCache()
        {
            if (_cache.TryGetValue("NumeroSolicitudes", out int _))
            {
                _cache.Remove("NumeroSolicitudes");
            }
        }
        /// <summary>
        /// Verifica si el usuario tiene el rol "Jefe".
        /// </summary>
        /// <returns>True si el usuario tiene el rol "Jefe", False en caso contrario.</returns>
        private bool VerificarRolJefe()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claims = identity.Claims;
            return claims.Where(c => c.Type == ClaimTypes.Role).Any(r => r.Value == "Jefe");
        }

        /// <summary>
        /// Verifica si el usuario tiene el rol "Administrador".
        /// </summary>
        /// <returns>True si el usuario tiene el rol "Administrador", False en caso contrario.</returns>
        private bool VerificarRolAdmin()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claims = identity.Claims;
            return claims.Where(c => c.Type == ClaimTypes.Role).Any(r => r.Value == "Administrador");
        }

        /// <summary>
        /// Verifica si el usuario tiene el rol "Mantenedor de bitácora".
        /// </summary>
        /// <returns>True si el usuario tiene el rol "Mantenedor de bitácora", False en caso contrario.</returns>
        private bool VerificarRolBitacora()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claims = identity.Claims;
            return claims.Where(c => c.Type == ClaimTypes.Role).Any(r => r.Value == "Mantenedor de bitácora");
        }
    }
}
