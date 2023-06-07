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
using System.Collections;
using System.Globalization;
using System.Security.Claims;

namespace ProyectoSolveCore.Controllers
{
    [Authorize]
    public class SolicitudController : Controller
    {
        private readonly ModelData _context;
        private readonly ILogger<SolicitudController> _logger;
        private readonly IMemoryCache _cache;
        private const string FormatLong = "dd/MMMM/yy HH:mm";
        private const string FormatShort = "dd/MMMM/yyyy";

        public SolicitudController(ModelData context, IMemoryCache cache, ILogger<SolicitudController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }
        [Autorizar(10)]
        [TypeFilter(typeof(VerificarSolicitudes))]
        public async Task<IActionResult> VisualizarSolicitudes()
        {
            ViewBag.notfilter = false;
            if (!await _context.Solicitudes.AnyAsync())
            {
                ViewBag.notfilter = true;
                return View(new List<Solicitude>());
            }
            try
            {
                var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation).Include(s => s.IdConductorNavigation.IdUsuarioNavigation)
                        .Include(s => s.IdVehiculoNavigation.IdCategoriaNavigation).Include(s => s.Aprobaciones)
                        .OrderByDescending(s => s.FechaSolicitado)
                        .ToListAsync();

                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text");
                ViewBag.Destino = new SelectList(ObtenerDestinos(solicitudes), "Value", "Text");
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(solicitudes), "Value", "Text", null, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(solicitudes), "Value", "Text");
                //ViewBag.OpcionFiltroFecha = new SelectList(ObtenerOpcionesFiltrosFecha(), "Value", "Text");
                //ViewBag.TipoFecha = 1; //Se selecciona por defecto al fecha solicitado

                return View(solicitudes);
            }
            catch (Exception ex)
            {
                ViewBag.notfilter = true;
                return View(new List<Solicitude>());
            }
        }

        private List<SelectListItem> ObtenerOpcionesFiltrosFecha()
        {
            List<SelectListItem> sli = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = "1", Text = "Filtrar solicitudes creadas hoy" },
                new SelectListItem(){ Value = "2", Text = "Filtrar solicitudes creadas hace 24 horas" },
                new SelectListItem(){ Value = "3", Text = "Filtrar solicitudes creadas esta semana" },
                new SelectListItem(){ Value = "4", Text = "Filtrar solicitudes creadas hace 7 dias" },
                new SelectListItem(){ Value = "5", Text = "Filtrar solicitudes creadas este mes" },
                new SelectListItem(){ Value = "6", Text = "Filtrar solicitudes creadas hace un mes"},
                new SelectListItem(){ Value = "7", Text = "Filtrar solicitudes creadas este año" },
                new SelectListItem(){ Value = "8", Text = "Filtrar solicitudes creadas hace 1 año" }
            };
            return sli;
        }

        [Autorizar(10)]
        [HttpPost]
        public async Task<IActionResult> VisualizarSolicitudes(vmFiltrosSolicitudes fs)
        {
            try
            {
                var NewList = await FiltrarSolicitudes(fs);

                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text", fs.Estado);
                ViewBag.Destino = new SelectList(ObtenerDestinos(NewList), "Value", "Text", fs.Destino);
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(NewList), "Value", "Text", fs.Vehiculo, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(NewList), "Value", "Text", fs.IdSolicitado);
                //ViewBag.FechaDesde = fs.FechaDesde;
                //ViewBag.FechaHasta = fs.FechaHasta;
                //ViewBag.Opcion = fs.Opcion;

                return View(NewList);
            }
            catch (Exception ex)
            {
                var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation).Include(s => s.IdVehiculoNavigation)
                        .ToListAsync();
                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text", fs.Estado);
                ViewBag.Destino = new SelectList(ObtenerDestinos(solicitudes), "Value", "Text", fs.Destino);
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(solicitudes), "Value", "Text", fs.Vehiculo, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(solicitudes), "Value", "Text", fs.IdSolicitado);
                //ViewBag.FechaDesde = fs.FechaDesde;
                //ViewBag.FechaHasta = fs.FechaHasta;
                //ViewBag.Opcion = fs.Opcion;
                return View(solicitudes);
            }
        }

        //Este metodo funciona para gestionar las solicitudes que estan aprobadas y listas para agregar a la bitacora
        public async Task<IActionResult> HubSolicitudes()
        {
            EliminarMemoriaCache();
            try
            {
                MemoryCacheEntryOptions opciones = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                int id = 0;
                var slist = await _context.Solicitudes.Include(s => s.IdConductorNavigation.IdUsuarioNavigation).Include(s => s.IdVehiculoNavigation)
                    .OrderBy(s => s.FechaSolicitado).Where(s => s.Estado == 1 && s.FechaLlegada >= DateTime.Now).ToListAsync();
                if (slist.Count < 1)
                {
                    return RedirectToAction("MisSolicitudes", "Solicitud");
                }
                if (VerificarRolAdmin() || VerificarRolBitacora()) //Con esto recuperamos todas las solicitudes listas para agregar a la bitacora
                {
                    _cache.Set("NumeroSolicitudes", slist.Count, opciones);
                    id = slist.FirstOrDefault().Id;
                    return RedirectToAction("AgregarEntradasBitacora", "Bitacora", new
                    {
                        id
                    });
                }
                int idUsuario = Convert.ToInt32(User.FindFirst("Id").Value);
                var slist_user = slist.Where(s => s.IdSolicitante == idUsuario).ToList();
                if (slist_user.Count < 1)
                {
                    return RedirectToAction("MisSolicitudes", "Solicitud");
                }
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
                //ViewBag.FechaDesde = solicitudes.Min(s => s.FechaSolicitado);
                //ViewBag.FechaHasta = solicitudes.Max(s => s.FechaLlegada);
                //ViewBag.Opcion = 1; //Se selecciona por defecto al fecha solicitado

                //listSolicitudes = await VerificarSolicitudesAtrasadas(listSolicitudes); 
                return View(listSolicitudes);
            }
            catch (Exception ex)
            {
                ViewBag.notfilter = true;
                return View(new List<vmSolicitud>());
            }
        }
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
                //ViewBag.FechaDesde = fs.FechaDesde;
                //ViewBag.FechaHasta = fs.FechaHasta;
                //ViewBag.Opcion = fs.Opcion;

                return View(misolicitudes);
            }
            catch (Exception)
            {
                ViewBag.notfilter = true;
                return View(new List<vmSolicitud>());
            }
        }

        //METODO EL CUAL OBTIENE TODAS LAS SOLICITUDES PARA MOSTRARLAS EN EL CALENDAR
        public async Task<JsonResult> GetSolicitudes() 
        {
            CultureInfo.CurrentCulture = new CultureInfo("es-CL");
            try
            {
                int year = DateTime.Now.Year;
                DateTime primerDiaDelAnio = new(year, 1, 1);
                DateTime ultimoDiaDelAnio = new(year, 12, 31);

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
                //var solicitudesNew = await VerificarSolicitudesAtrasadas(solicitudes);
                return Json(solicitudes);
            }
            catch (Exception)
            {
                return Json(new
                {
                    mensaje = "Ocurrio un error inesperado, contacte con administrador del sitio o intente mas tarde",
                    type = "danger"
                });
            }
        }
        public async Task<PartialViewResult> MasInformacionSolicitud(int? id, bool aprobacion = true)
        {
            try
            {
                string mensaje = string.Empty;
                if (!id.HasValue)
                {
                    mensaje = "Hubo un error al recibir los datos, recargue la página o intentelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }

                var solicitud = await _context.Solicitudes
                    .Include(s => s.IdVehiculoNavigation).Include(s => s.IdSolicitanteNavigation)
                    .Include(s => s.IdConductorNavigation.IdUsuarioNavigation).Include(s => s.Aprobaciones)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (solicitud == null)
                {
                    mensaje = "Hubo un error al cargar los datos, recargue la página o intentelo nuevamente";
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
                string mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }
        [Authorize(Roles = "Adminstrador, Solicitador")]
        public async Task<IActionResult> SolicitarVehiculo()
        {
            try
            {
                int idUser = int.Parse(User.FindFirst("Id").Value);
                var departamentos = await _context.Departamentos.ToListAsync();
                var usuarios = await _context.Usuarios.Where(u => !u.Eliminado && u.Id != 1 && u.Id != 2)
                .Select(u => new vmUsuarioDepartamento()
                {
                    Id = u.Id,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    IdDepartamento = u.IdDepartamento
                })
                .ToListAsync();
                var usuario = usuarios.FirstOrDefault(u => u.Id == idUser);
                ViewBag.IdSolicitante = new SelectList(usuarios, "Id", "NombreCompleto", idUser);
                ViewBag.Departamento = await _context.Departamentos.FirstOrDefaultAsync(d => d.Id == usuario.IdDepartamento);
                return View(new Solicitude()
                {
                    IdSolicitante = idUser
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("MisSolicitudes");
            }
        }
        public async Task<JsonResult> SelectGetDepartamento(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return Json(new
                    {
                        Mensaje = "Ocurrio un error al recibir los datos",
                        Departamento = string.Empty,
                    });
                }
                var usuario = await _context.Usuarios.Include(u => u.IdDepartamentoNavigation)
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (usuario == null)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrio un error al obtener los datos",
                        Departamento = string.Empty
                    });
                }
                string departamento = usuario.IdDepartamentoNavigation.Departamento1;
                return Json(new
                {
                    Mensaje = "Departamento obtenido exitosamento",
                    Departamento = departamento
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Mensaje = "Ocurrio un error innesperado. Avise al administrador del sistema o intentelo nuevamente",
                    Departamento = string.Empty
                });
            }
        }
        [Authorize(Roles = "Adminstrador, Solicitador")]
        [HttpPost]
        public async Task<IActionResult> SolicitarVehiculo(Solicitude solicitud)
        {
            //CultureInfo.CurrentCulture = new CultureInfo("es-CL");
            _logger.LogInformation($"Fecha Salida: {solicitud.FechaSalida} Fecha Llegada: {solicitud.FechaLlegada}");
            try
            {
                if (solicitud == null)
                {
                    return View(new Solicitude());
                }
                
                _logger.LogInformation($"Fecha solicitado: {solicitud.FechaSolicitado}");

                var vehiculo = await _context.Vehiculos.FirstOrDefaultAsync(v => v.Id == solicitud.IdVehiculo);
                // Convertir el JSON a una lista de objetos
                List<PasajerosAux> valueList = JsonConvert.DeserializeObject<List<PasajerosAux>>(solicitud.Pasajeros);
                //Unir todo a cadena de texto
                solicitud.Pasajeros = string.Join(", ", valueList.ConvertAll(x => x.value));
                solicitud.FechaSolicitado = DateTime.Now;
                if (vehiculo.IdConductor.HasValue)
                {
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
                _logger.LogInformation(solicitud.IdSolicitante.ToString());
                _logger.LogError(ex.Message);
                return View(new Solicitude());
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetVehiculos([FromBody] vmFechaSalidaLlegada datos)
        {
            try
            {
                if (!DateTime.TryParse(datos.fecha_salida, out DateTime fecha_salida)
                || !DateTime.TryParse(datos.fecha_llegada, out DateTime fecha_llegada))
                {
                    return Json(new
                    {
                        data = new List<SelectListItem>(),
                        mensaje = "Al parecer las fechas seleccionadas son incorrectas o algunos campos están vacíos, contacte con el administrador del sitio o intente más tarde",
                        type = "danger"
                    });
                }

                var solicitudes = await _context.Solicitudes
                    .Where(s => s.Estado < 2 && (s.FechaSalida <= fecha_salida && s.FechaLlegada >= fecha_salida)
                                             || (s.FechaLlegada >= fecha_llegada && s.FechaSalida <= fecha_llegada))
                    .ToListAsync();

                if (solicitudes.Count == 0)
                {
                    return Json(new
                    {
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

                var vehiculosIdsOcupados = solicitudes.Select(s => s.IdVehiculo).ToList();

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
                    mensaje = "Ocurrio un error inesperado, contacte con administrador del sitio o intente mas tarde: "+ex.Message,
                    type = "danger"
                });
            }
        }
        //SOLICITUDES PENDIENTES
        [TypeFilter(typeof(VerificarSolicitudes))]
        public async Task<IActionResult> SolicitudesPendientes()
        {
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
                    return View(new List<Solicitude>());
                }

                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text");
                ViewBag.Destino = new SelectList(ObtenerDestinos(solicitudes), "Value", "Text");
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(solicitudes), "Value", "Text", null, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(solicitudes), "Value", "Text");
                //ViewBag.FechaDesde = solicitudes.Min(s => s.FechaSolicitado);
                //ViewBag.FechaHasta = solicitudes.Max(s => s.FechaLlegada);
                //ViewBag.Opcion = 1; //Se selecciona por defecto al fecha solicitado

                return View(solicitudes);
            }
            catch (Exception ex)
            {
                ViewBag.notfilter = true;
                return View(new List<Solicitude>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SolicitudesPendientes(vmFiltrosSolicitudes fs)
        {
            try
            {
                var solicitudes = await FiltrarSolicitudes(fs);
                solicitudes = solicitudes.Where(s => s.Estado == 0).ToList();

                ViewBag.Estado = new SelectList(ObtenerEstados(), "Value", "Text", fs.Estado);
                ViewBag.Destino = new SelectList(ObtenerDestinos(solicitudes), "Value", "Text", fs.Destino);
                ViewBag.Vehiculo = new SelectList(ObtenerVehiculo(solicitudes), "Value", "Text", fs.Vehiculo, "Group");
                ViewBag.IdSolicitado = new SelectList(ObtenerUsuarios(solicitudes), "Value", "Text", fs.IdSolicitado);
                //ViewBag.FechaDesde = fs.FechaDesde;
                //ViewBag.FechaHasta = fs.FechaHasta;
                //ViewBag.Opcion = fs.Opcion;

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
                //ViewBag.FechaDesde = fs.FechaDesde;
                //ViewBag.FechaHasta = fs.FechaHasta;
                //ViewBag.Opcion = fs.Opcion;

                return View(solicitudes);
            }
        }
        [Authorize(Roles = "Administrador, Jefe")]
        public async Task<PartialViewResult> AsignarConductor(int? id)
        {
            try
            {
                var solicitud = await _context.Solicitudes.Include(s => s.IdVehiculoNavigation).Include(s => s.IdConductorNavigation.IdUsuarioNavigation).Include(s => s.IdSolicitanteNavigation)
                    .FirstOrDefaultAsync(s => s.Id == id);
                Usuario usuario = solicitud.IdSolicitanteNavigation;
                List<vmConductoresList> conductores = new();
                int ConductorAsignado = solicitud.IdVehiculoNavigation.IdConductor??0;
                if (!solicitud.IdConductor.HasValue)
                {
                    conductores = await GetConductoresDisponibles(solicitud);
                }
                conductores = await _context.Conductores.Include(c => c.IdUsuarioNavigation)
                    .Where(c => c.Id != id && !c.IdUsuarioNavigation.Eliminado && c.Estado).Select(c => new vmConductoresList()
                    {
                        Id = c.Id,
                        Nombre = c.IdUsuarioNavigation.Nombre + " " + c.IdUsuarioNavigation.Apellido,
                        rut = c.IdUsuarioNavigation.Rut
                    }).ToListAsync();
                ViewBag.NombreSolicitante = usuario.Nombre + " " + usuario.Apellido;
                ViewBag.IdConductor = new SelectList(conductores, "Id", "Nombre", (ConductorAsignado != 0) ? ConductorAsignado : null);
                return PartialView("_AsignarConductor", solicitud);
            }
            catch (Exception)
            {
                string mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }
        [Authorize(Roles = "Administrador, Jefe")]
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
                        mensaje = "Hubo un error al enviar los datos. Asegurese que haya seleccionado un conductor.",
                        type = "danger"
                    });
                }
                using var transaction = await _context.Database.BeginTransactionAsync();
                var solicitud = await _context.Solicitudes.FindAsync(ids.IdSolicitud);
                if (solicitud == null)
                {
                    return Json(new
                    {
                        mensaje = "Hubo un error al obtener los datos, intentelo nuevamente",
                        type = "danger"
                    });
                }

                solicitud.IdConductor = ids.IdConductor;

                var aprobacion = new Aprobacione()
                {
                    Fecha = DateTime.Now,
                    Estado = true,
                    Motivo = null,
                    IdJefe = id,
                    IdSolicitud = ids.IdSolicitud
                };

                await _context.Aprobaciones.AddAsync(aprobacion);
                int n = await _context.SaveChangesAsync();
                //Se cambia el estado a la solicitud a aprobada
                solicitud.Estado++;
                n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    return Json(new
                    {
                        mensaje = "Hubo un error al guardar los datos, intentelo nuevamente",
                        type = "danger"
                    });
                }
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
                    mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente",
                    type = "danger"
                });
            }
        }

        [Authorize(Roles = "Administrador, Jefe")]
        public async Task<PartialViewResult> ConfirmacionRechazar(int id = 0)
        {
            try
            {
                string mensaje = string.Empty;
                if (id == 0)
                {
                    mensaje = "Hubo un error al recibir los datos, recargue la página o intenlo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }
                var solicitud = await _context.Solicitudes.Include(s => s.IdVehiculoNavigation)
                    .FirstOrDefaultAsync(s => s.Id == id);
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == solicitud.IdSolicitante);
                ViewBag.NombreSolicitante = usuario.Nombre + " " + usuario.Apellido;

                return PartialView("_ConfirmarRechazar", solicitud);
            }
            catch (Exception)
            {
                string mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }
        [Authorize(Roles = "Administrador, Jefe")]
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
                int id = Convert.ToInt32(User.FindFirst("Id").Value);

                using var transaction = await _context.Database.BeginTransactionAsync();
                var solicitud = await _context.Solicitudes.FindAsync(datos.id_solicitud);
                if (solicitud == null)
                {
                    return Json(new
                    {
                        mensaje = "Hubo un error al obtener los datos, intentelo nuevamente",
                        type = "danger"
                    });
                }

                var aprobacion = new Aprobacione()
                {
                    Fecha = DateTime.Now,
                    Estado = false,
                    Motivo = datos.motivo,
                    IdJefe = id,
                    IdSolicitud = datos.id_solicitud
                };

                await _context.Aprobaciones.AddAsync(aprobacion);
                solicitud.Estado = 2;
                int n = await _context.SaveChangesAsync();
                
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
                    mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente",
                    type = "danger"
                });
            }
        }
        private async Task<List<vmConductoresList>> GetConductoresDisponibles(Solicitude solicitud)
        {
            List<int?> ConductoresOcupadosId = new();
            List<vmConductoresList> conductoresDisponibles = new();

            var SolicitudesInSolicitud = await _context.Solicitudes
                .Where(s => (s.FechaSalida <= solicitud.FechaSalida && s.FechaLlegada >= solicitud.FechaLlegada)
                        || (s.FechaLlegada >= solicitud.FechaLlegada && s.FechaSalida <= solicitud.FechaLlegada))
                .Where(s => s.Estado < 2 && s.IdConductor.HasValue).ToListAsync();

            if (SolicitudesInSolicitud.Count == 0)
            {
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

            ConductoresOcupadosId = SolicitudesInSolicitud.Select(s => s.IdConductor).ToList();
            foreach (var id in ConductoresOcupadosId)
            {
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
        private static List<SelectListItem> ObtenerEstados()
        {
            List<SelectListItem> list = new();
            Dictionary<int, string> EstadosDicc = new()
            {
                { 0, "Pendiente" },
                { 1, "Aprobada" },
                { 2, "Rechazada" },
                { 3, "Finalizada" },
            };

            foreach (var key in EstadosDicc.Keys)
            {
                var e = EstadosDicc[key];
                list.Add(new SelectListItem
                {
                    Value = key.ToString(),
                    Text = e
                });
            }
            return list;
        }
        private static List<SelectListItem> ObtenerDestinos(List<Solicitude> solicitudes)
        {
            List<SelectListItem> list = new();
            var destinos = solicitudes.Select(s => s.Destino)
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var d in destinos)
            {
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
        private static List<SelectListWithGroups> ObtenerVehiculo(List<Solicitude> solicitudes)
        {
            List<SelectListWithGroups> list = new();
            var vehiculos = solicitudes.Select(s => s.IdVehiculoNavigation)
                .Distinct().Where(v => !v.Eliminado).ToList();
            foreach (var v in vehiculos)
            {
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
        private static List<SelectListItem> ObtenerUsuarios(List<Solicitude> solicitudes)
        {
            List<SelectListItem> list = new();
            var usuarios = solicitudes.Select(s => s.IdSolicitanteNavigation).Distinct().ToList();

            foreach (var u in usuarios)
            {
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
        private async Task<List<Solicitude>> FiltrarSolicitudes(vmFiltrosSolicitudes fs)
        {
            var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation).Include(s => s.IdConductorNavigation.IdUsuarioNavigation)
                        .Include(s => s.IdVehiculoNavigation.IdCategoriaNavigation).Include(s => s.Aprobaciones)
                        .OrderByDescending(s => s.FechaSolicitado)
                        .ToListAsync();

            if (fs == null
                || (fs.Estado == -1 && string.IsNullOrEmpty(fs.Destino) && string.IsNullOrEmpty(fs.Vehiculo)))//&& fs.FechaDesde == DateTime.MinValue
            {
                return solicitudes;
            }
            if (fs.Estado > -1)
            {
                solicitudes = solicitudes.Where(s => s.Estado == fs.Estado).ToList();
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
            //if (fs.FechaDesde != DateTime.MinValue)
            //{
            //    switch (fs.Opcion)
            //    {
            //        case 1:
            //            solicitudes = solicitudes.Where(s => s.FechaSolicitado >= fs.FechaDesde).ToList();
            //            break;
            //        case 2:
            //            solicitudes = solicitudes.Where(s => s.FechaSalida >= fs.FechaDesde).ToList();
            //            break;
            //        case 3:
            //            solicitudes = solicitudes.Where(s => s.FechaLlegada >= fs.FechaDesde).ToList();
            //            break;
            //    }
            //}
            //if (fs.FechaHasta != DateTime.MinValue)
            //{
            //    switch (fs.Opcion)
            //    {
            //        case 1:
            //            solicitudes = solicitudes.Where(s => s.FechaSolicitado >= fs.FechaDesde && s.FechaSolicitado <= fs.FechaHasta).ToList();
            //            break;
            //        case 2:
            //            solicitudes = solicitudes.Where(s => s.FechaSalida >= fs.FechaDesde && s.FechaSalida <= fs.FechaHasta).ToList();
            //            break;
            //        case 3:
            //            solicitudes = solicitudes.Where(s => s.FechaLlegada >= fs.FechaDesde && s.FechaLlegada <= fs.FechaHasta).ToList();
            //            break;
            //    }
            //}
            return solicitudes;
        }
        private void EliminarMemoriaCache()
        {
            if (_cache.TryGetValue("NumeroSolicitudes", out int _))
            {
                _cache.Remove("NumeroSolicitudes");
            }
        }
        private bool VerificarRolJefe()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claims = identity.Claims;
            return claims.Where(c => c.Type == ClaimTypes.Role).Any(r => r.Value == "Jefe");
        }
        private bool VerificarRolAdmin()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claims = identity.Claims;
            return claims.Where(c => c.Type == ClaimTypes.Role).Any(r => r.Value == "Administrador");
        }
        private bool VerificarRolBitacora()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claims = identity.Claims;
            return claims.Where(c => c.Type == ClaimTypes.Role).Any(r => r.Value == "Mantenedor de bitácora");
        }
        //Metodo que verifica si la aprobacion tiene 2 aprobaciones por parte de los jefes
        //private string VerificarAprobacion(int id)
        //{
        //    try
        //    {
        //        var aprobaciones = _context.Aprobaciones.Where(a => a.IdSolicitud == id).ToList();
        //        if (aprobaciones.Any())
        //        {
        //            var numAprobacionRechazo = aprobaciones.Where(a => !a.Estado).ToList().Count;
        //            if (numAprobacionRechazo > 0)
        //            {
        //                return numAprobacionRechazo > 1 ? "Rechazado" : "Pendiente";
        //            }
        //            var numAprobacionPositiva = aprobaciones.Where(a => a.Estado).ToList().Count;
        //            return numAprobacionPositiva > 1 ? "Aprobado" : "Pendiente";
        //        }
        //        return "Pendiente";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error";
        //    }
        //}
    }
}
