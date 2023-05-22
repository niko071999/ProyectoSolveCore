using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;

namespace ProyectoSolveCore.Controllers
{
    [Authorize]
    public class SolicitudController : Controller
    {
        private readonly ModelData _context;
        private const string FormatLong = "dd/MMMM/yy HH:mm";
        private const string FormatShort = "MMMM/dd/yyyy";

        public SolicitudController(ModelData context)
        {
            _context = context;
        }
        [TypeFilter(typeof(VerificarSolicitudes))]
        public async Task<IActionResult> VisualizarSolicitudes()
        {
            var solicitudes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation).Include(s => s.IdVehiculoNavigation)
                .OrderByDescending(s => s.FechaSolicitado)
                .ToListAsync();
            //solicitudes = await VerificarSolicitudesAtrasadas(solicitudes);
            return View(solicitudes);
        }
        
        //Este metodo funciona para gestionar las solicitudes que estan aprobadas y listas para agregar a la bitacora
        public IActionResult HubSolicitudes()
        {
            try
            {
                int idUsuario = Convert.ToInt32(User.FindFirst("Id").Value);

                var solicitud = _context.Solicitudes.Include(s => s.IdConductorNavigation.IdUsuarioNavigation).Include(s => s.IdVehiculoNavigation)
                    .OrderBy(s => s.FechaSolicitado)
                    .FirstOrDefault(s => s.Estado == 1 && s.IdSolicitante == idUsuario);

                if (solicitud == null)
                {
                    return RedirectToAction("MisSolicitudes", "Solicitud");
                }
                int id = solicitud.Id;
                return RedirectToAction("AgregarEntradasBitacora", "Bitacora", new
                {
                    id
                });
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [TypeFilter(typeof(VerificarSolicitudes))]
        public async Task<IActionResult> MisSolicitudes()
        {
            try
            {
                int Id = Convert.ToInt32(User.FindFirst("Id").Value);
                var listSolicitudes = await _context.Solicitudes.Include(s => s.IdVehiculoNavigation).Include(s => s.Aprobaciones)
                        .Include(s => s.IdConductorNavigation.IdUsuarioNavigation).OrderByDescending(s => s.FechaSolicitado)
                        .Where(s => s.IdSolicitante == Id)
                        .Select(s => new vmSolicitud()
                        {
                            id = s.Id,
                            FechaSolicitado = s.FechaSolicitado,
                            FechaLlegada = s.FechaLlegada,
                            FechaSalida = s.FechaSalida,
                            Estado = s.Estado,
                            vehiculo = s.IdVehiculoNavigation.Patente + " - " + s.IdVehiculoNavigation.Marca + " " + s.IdVehiculoNavigation.Modelo,
                            nombreConductor = s.IdConductorNavigation.IdUsuarioNavigation.Nombre + " " + s.IdConductorNavigation.IdUsuarioNavigation.Apellido,
                            CantidadAprobacion = s.Aprobaciones.Count
                        }).ToListAsync();
                //listSolicitudes = await VerificarSolicitudesAtrasadas(listSolicitudes); 
                return View(listSolicitudes);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        //METODO EL CUAL OBTIENE TODAS LAS SOLICITUDES PARA MOSTRARLAS EN EL CALENDAR
        public async Task<JsonResult> GetSolicitudes() 
        {
            try
            {
                DateTime fechaActual = DateTime.Now;
                DateTime primerDiaDelAnio = new(fechaActual.Year, 1, 1);
                DateTime ultimoDiaDelAnio = new(fechaActual.Year, 12, 31);

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
                    .Include(s => s.IdVehiculoNavigation).Include(s => s.IdSolicitanteNavigation).Include(s => s.IdConductorNavigation.IdUsuarioNavigation)
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
        public IActionResult SolicitarVehiculo()
        {
            int idUser = Convert.ToInt32(User.FindFirst("Id").Value);
            try
            {
                ViewBag.Usuario = _context.Usuarios
                .Where(u => u.Id == idUser)
                .Select(u => new vmUsuarioDepartamento()
                {
                    Id = idUser,
                    NombreCompleto = User.Identity.Name,
                    Departamento = User.FindFirst("Departamento").Value
                })
                .FirstOrDefault();
                return View(new Solicitude());
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
        [Authorize(Roles = "Adminstrador, Solicitador")]
        [HttpPost]
        public async Task<IActionResult> SolicitarVehiculo(Solicitude solicitud)
        {
            try
            {
                if (solicitud == null)
                {
                    return NotFound();
                }
                var hoy = DateTime.Now;
                if (solicitud.FechaLlegada <= hoy || solicitud.FechaSalida <= hoy
                    || solicitud.FechaSalida >= solicitud.FechaLlegada)
                {
                    ViewBag.Usuario = await _context.Usuarios.Select(u => new vmUsuarioDepartamento()
                    {
                        Id = Convert.ToInt32(User.FindFirst("Id").Value),
                        NombreCompleto = User.Identity.Name,
                        Departamento = User.FindFirst("Departamento").Value
                    }).FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(User.FindFirst("Id").Value));
                    return View(solicitud);
                }

                // Convertir el JSON a una lista de objetos
                List<PasajerosAux> valueList = JsonConvert.DeserializeObject<List<PasajerosAux>>(solicitud.Pasajeros);
                //Unir todo a cadena de texto
                solicitud.Pasajeros = string.Join(", ", valueList.ConvertAll(x => x.value));

                solicitud.FechaSolicitado = hoy;
                _context.Solicitudes.Add(solicitud);
                int n = await _context.SaveChangesAsync();

                if (n > 0)
                {
                    return RedirectToAction(nameof(MisSolicitudes));
                }
                return View(solicitud);
            }
            catch (Exception ex)
            {
                return NotFound();
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
                        mensaje = "Ocurrió un error inesperado, contacte con el administrador del sitio o intente más tarde",
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
            catch (Exception)
            {
                return Json(new
                {
                    mensaje = "Ocurrio un error inesperado, contacte con administrador del sitio o intente mas tarde",
                    type = "danger"
                });
            }
        }
        //SOLICITUDES PENDIENTES
        [TypeFilter(typeof(VerificarSolicitudes))]
        public async Task<IActionResult> SolicitudesPendientes()
        {
            try
            {
                int id = Convert.ToInt32(User.FindFirst("Id").Value);
                var solicitudesPendientes = await _context.Solicitudes.Include(s => s.IdSolicitanteNavigation).Include(s => s.IdVehiculoNavigation)
                    .Where(s => s.Estado == 0)
                    .ToListAsync();

                var aprobacionesSolicitud = await _context.Aprobaciones
                    .Where(a => a.IdJefe == id)
                    .ToListAsync();

                solicitudesPendientes.RemoveAll(sp => aprobacionesSolicitud.Any(aS => aS.IdSolicitud == sp.Id));

                return View(solicitudesPendientes);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
        [Authorize(Roles = "Administrador, Jefe")]
        public async Task<PartialViewResult> AsignarConductor(int? id)
        {
            try
            {
                var solicitud = await _context.Solicitudes.Include(s => s.IdVehiculoNavigation).Include(s => s.IdConductorNavigation.IdUsuarioNavigation)
                    .FirstOrDefaultAsync(s => s.Id == id);
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == solicitud.IdSolicitante);
                List<vmConductoresList> conductores = new();
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
                ViewBag.IdConductor = new SelectList(conductores, "Id", "Nombre");
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
            try
            {
                int id = Convert.ToInt32(User.FindFirst("Id").Value);
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
                string estadoAprobacion = VerificarAprobacion(solicitud.Id);
                if (estadoAprobacion.Equals("Aprobado"))
                {
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
                if (estadoAprobacion.Equals("Rechazado"))
                {
                    solicitud.Estado = 2;
                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                        mensaje = "Se a guardado correctamente su aprobación",
                        type = "success"
                    });
                }
                await transaction.CommitAsync();
                return Json(new
                {

                    mensaje = "Se a aprobado la solicitud satisfactoriamente",
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
                    Motivo = null,
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
        private string VerificarAprobacion(int id)
        {
            try
            {
                var aprobaciones = _context.Aprobaciones.Where(a => a.IdSolicitud == id).ToList();
                if (aprobaciones.Any())
                {
                    var numAprobacionRechazo = aprobaciones.Where(a => !a.Estado).ToList().Count;
                    if (numAprobacionRechazo > 0)
                    {
                        return numAprobacionRechazo > 1 ? "Rechazado" : "Pendiente";
                    }
                    var numAprobacionPositiva = aprobaciones.Where(a => a.Estado).ToList().Count;
                    return numAprobacionPositiva > 1 ? "Aprobado" : "Pendiente";
                }
                return "Pendiente";
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }
    }
}
