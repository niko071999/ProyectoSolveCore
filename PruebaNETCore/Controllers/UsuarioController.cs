using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;
using PruebaNETCore.Models;
using System.Linq;
using System.Security.Claims;

namespace ProyectoSolveCore.Controllers
{
    [Authorize(Roles = "Administrador, Mantenedor usuarios")]
    public class UsuarioController : Controller
    {
        private readonly ModelData _context;
        public UsuarioController(ModelData context)
        {
            _context = context;
        }
        [Authorize(Roles = "Administrador, Mantenedor usuarios, Jefe")]
        public IActionResult VisualizarUsuarios()
        {
            try
            {
                var usuarios = _context.Usuarios.Where(u => !u.Eliminado).Select(u => new vmUsuario()
                {
                    Id = u.Id,
                    Rut = u.Rut,
                    Nombre = u.Nombre + " " + u.Apellido,
                    Departamento = u.IdDepartamentoNavigation.Departamento1,
                    Roles = u.UsuariosRoles.Select(ur => ur.IdRolNavigation).Select(r => new vmRol()
                    {
                        Id = r.Id,
                        rol = r.Rol,
                        IconRol = ObtenerIconRol(r.Id)
                    }).ToList(),
                    Permisos = u.UsuariosRoles.Select(ur => ur.IdRolNavigation).SelectMany(r => r.RolesPermisos).Select(rp => rp.IdPermisoNavigation).Select(p => new VmPermiso()
                    {
                        Permiso = p.Permiso1,
                        IconPermiso = ObtenerIconPermiso(p.Id)
                    }).ToList(),
                    Conductor = u.Conductores.Any()
                }).ToList();

                return View(usuarios);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        public ActionResult AgregarUsuario()
        {
            return View(new vmUsuarioConductorRoles());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarUsuario(vmUsuarioConductorRoles uc)
        {
            if (uc == null)
            {
                return View(new vmUsuarioConductorRoles());
            }
            var userAny = await _context.Usuarios.Where(u => !u.Eliminado).AnyAsync(u => u.Rut == uc.rut);
            if (userAny)//ERROR: Verifica si existe otro usuario con el mismo rut
            {
                return View(uc);
            }
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                var usuario = new Usuario()
                {
                    Rut = uc.rut,
                    Nombre = uc.nombre,
                    Apellido = uc.apellido,
                    Login = uc.login,
                    Clave = Encrypt.EncryptPassword(uc.clave),
                    IdDepartamento = uc.id_departamento,
                    DireccionImg = "/assets/sin-foto.png",
                    Eliminado = false
                };

                await _context.Usuarios.AddAsync(usuario);
                int n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    return View(uc);
                }
                uc.ID = usuario.Id;
                var UsuarioRole = ObtenerRoles(uc).Where(ur => ur.check).Select(ur => new UsuariosRole()
                {
                    IdRol = ur.IdRol,
                    IdUsuario = ur.IdUsuario
                });

                await _context.UsuariosRoles.AddRangeAsync(UsuarioRole);
                await _context.SaveChangesAsync();

                // VERIFICA SI EXISTEN DATOS DEL CONDUCTOR, SI ESTÁN NULOS GUARDA EN BASE DE DATOS LOS DATOS DE USUARIO
                if (uc.FechaEmitida == null || uc.FecheVencimiento == null || uc.NumeroPoliza == null)
                {
                    if (n == 0)
                    {
                        return View(uc);
                    }

                    await transaction.CommitAsync();
                    return RedirectToAction(nameof(VisualizarUsuarios));
                }
                //Se agrega el rol de conductor
                await _context.UsuariosRoles.AddAsync(new UsuariosRole()
                {
                    IdRol = 6,
                    IdUsuario = usuario.Id
                });
                await _context.SaveChangesAsync();

                var conductor = new Conductore()
                {
                    Estado = true,
                    FechaEmision = (DateTime)uc.FechaEmitida,
                    FechaVencimiento = (DateTime)uc.FecheVencimiento,
                    IdUsuario = usuario.Id,
                    NumeroPoliza = (int)uc.NumeroPoliza,
                };

                _context.Conductores.Add(conductor);
                n = await _context.SaveChangesAsync();

                if (n == 0)
                {
                    return View(uc);
                }

                await transaction.CommitAsync();
                return RedirectToAction("VisualizarUsuarios");
            }
            catch (Exception ex)
            {
                return View(uc);
            }
        }
        public async Task<PartialViewResult> EditarUsuario(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    string mensaje = "Hubo un error al recibir los datos";
                    return PartialView("_PartialModalError", mensaje);
                }
                var usuario = await _context.Usuarios.Where(u => !u.Eliminado && u.Id == id).Select(u => new vmUsuarioConductorRoles()
                {
                    ID = u.Id,
                    rut = u.Rut,
                    nombre = u.Nombre,
                    apellido = u.Apellido,
                    id_departamento = u.IdDepartamentoNavigation.Id,
                    direccion_img = u.DireccionImg,
                    id_conductor = u.Conductores.Any() ? u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).Id:null,
                    NumeroPoliza = u.Conductores.Any() ? u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).NumeroPoliza : null,
                    FechaEmitida = u.Conductores.Any() ? u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).FechaEmision : null,
                    FecheVencimiento = u.Conductores.Any() ? u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).FechaVencimiento : null,
                    RolAdministrador = u.UsuariosRoles.Any(ur => ur.IdUsuario == u.Id && ur.IdRol == 1),
                    RolJefe = u.UsuariosRoles.Any(ur => ur.IdUsuario == u.Id && ur.IdRol == 2),
                    RolMantenedorVehiculos = u.UsuariosRoles.Any(ur => ur.IdUsuario == u.Id && ur.IdRol == 3),
                    RolMantendorUsuarios = u.UsuariosRoles.Any(ur => ur.IdUsuario == u.Id && ur.IdRol == 4),
                    RolSolicitador = u.UsuariosRoles.Any(ur => ur.IdUsuario == u.Id && ur.IdRol == 5)
                }).FirstOrDefaultAsync();
                if (usuario == null)
                {
                    return PartialView("_PartialModalError", usuario);
                }
                ViewBag.id_departamento = new SelectList(_context.Departamentos.ToList(), "Id", "Departamento1", usuario.id_departamento);
                return PartialView("_EditarUsuario", usuario);
            }
            catch (Exception ex)
            {
                string mensaje = "Hubo un error inesperado: "+ex.Message;
                return PartialView("_PartialModalError",mensaje);
            }
        }
        [HttpPost]
        public async Task<JsonResult> EditarUsuario(vmUsuarioConductorRoles uc)
        {
            if (uc == null)
            {
                return Json(new
                {
                    mensaje = "Ocurrio un error al recibir los datos",
                    type = "error"
                });
            }
            var usuarios = await _context.Usuarios.Where(u => !u.Eliminado && u.Rut == uc.rut).ToListAsync();
            if (usuarios.Count > 1)
            {
                //Verifico si hay mas de 1 elemento con el rut del usuario
                return Json(new
                {
                    mensaje = "El rut pertenece a otro usuario registrado",
                    type = "error"
                });
            }
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                int contains = 0;//Cuenta si hubieron roles que se ingresaron a la base de datos para verificar

                var usuario = new Usuario()
                {
                    Id = uc.ID,
                    Rut = uc.rut,
                    Nombre = uc.nombre,
                    Apellido = uc.apellido,
                    Clave = !string.IsNullOrEmpty(uc.clave) ? Encrypt.EncryptPassword(uc.clave) : "",
                    IdDepartamento = uc.id_departamento,
                    DireccionImg = "",
                    Eliminado = false
                };
                if (string.IsNullOrEmpty(uc.clave))
                {
                    //Si la clave viene vacia o nula, no se modifica en la base de datos
                    _context.Entry(usuario).Property(u => u.Clave).IsModified = false;
                }
                else
                {
                    _context.Update(usuario).State = EntityState.Modified;
                }
                var UsuarioRole = ObtenerRoles(uc);
                var RolesUsuario = _context.UsuariosRoles.Where(ur => ur.IdUsuario == uc.ID).ToList();
                foreach (var ur in UsuarioRole)
                {
                    var usuarioRol = await _context.UsuariosRoles.FirstOrDefaultAsync(x => x.IdRol == UsuarioRole[contains].IdRol 
                                        && x.IdUsuario == UsuarioRole[contains].IdUsuario);

                    if (usuarioRol != null)
                    {
                        if (!UsuarioRole[contains].check)
                        {
                            _context.UsuariosRoles.Remove(usuarioRol); // Eliminar el registro
                        }
                        // No hacer nada si el rol existe y está seleccionado
                    }
                    else if (UsuarioRole[contains].check)
                    {
                        var nuevoUsuarioRol = new UsuariosRole()
                        {
                            IdUsuario = UsuarioRole[contains].IdUsuario,
                            IdRol = UsuarioRole[contains].IdRol
                        };
                        _context.UsuariosRoles.Add(nuevoUsuarioRol); // Agregar el nuevo registro
                    }
                    contains++;
                }
                // VERIFICA SI EXISTEN DATOS DEL CONDUCTOR, SI ESTÁN NULOS GUARDA EN BASE DE DATOS LOS DATOS DE USUARIO
                var rolConductor = _context.UsuariosRoles.FirstOrDefault(ur => ur.IdRol == 6 && ur.IdUsuario == uc.ID);
                if (uc.FechaEmitida == null || uc.FecheVencimiento == null || uc.NumeroPoliza == null)
                {
                    //Borrar rol conductor
                    if (rolConductor != null)
                    {
                        _context.Remove(rolConductor);
                    }
                    //Borrar conductor
                    var c = _context.Conductores.Find(uc.id_conductor);
                    if (c != null)
                    {
                        _context.Conductores.Remove(c);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Json(new
                    {
                        mensaje = "Usuario editado correctamente",
                        type = "success"
                    });
                }

                if (rolConductor == null)
                {
                    await _context.UsuariosRoles.AddAsync(new UsuariosRole()
                    {
                        IdRol = 6,
                        IdUsuario = uc.ID,
                    });
                }

                //Actualizar el conductor
                var conductor = new Conductore()
                {
                    Estado = true,
                    FechaEmision = (DateTime)uc.FechaEmitida,
                    FechaVencimiento = (DateTime)uc.FecheVencimiento,
                    IdUsuario = usuario.Id,
                    NumeroPoliza = (int)uc.NumeroPoliza,
                };
                _context.Conductores.Update(conductor);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Json(new
                {
                    mensaje = "Usuario editado correctamente",
                    type = "success"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    mensaje = "Ocurrio un error al recibir los datos",
                    type = "error"
                });
            }
        }
        private static List<vmRolCheck> ObtenerRoles(vmUsuarioConductorRoles uc)
        {
            List<vmRolCheck> lur = new();
            Dictionary<int, bool> roles = new()
            {
                { 1, uc.RolAdministrador },
                { 2, uc.RolJefe },
                { 3, uc.RolMantenedorVehiculos },
                { 4, uc.RolMantendorUsuarios },
                { 5, uc.RolSolicitador },
                { 7, uc.RolMantenedorVehiculosMaq },
                { 8, uc.RolMantenedorBitacora }
            };

            for (int i = 0; i < 5; i++)
            {
                lur.Add(new vmRolCheck { IdRol = i+1, IdUsuario = uc.ID, check = roles[i+1] });
            }
            return lur;
        }

        public async Task<PartialViewResult> EliminarUsuario(int id = 0)
        {
            try
            {
                string mensaje;
                if (id == 0)
                {
                    mensaje = "Hubo un error al recibir los datos, intentelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    mensaje = "Hubo un error al recibir los datos, intentelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }

                return PartialView("_BorrarUsuario", usuario);
            }
            catch (Exception)
            {
                string mensaje = "Ocurrio un error inesperado, consulte con administrador del sistema o intentelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
        }
        [HttpPost]
        public async Task<JsonResult> BorrarUsuario(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrio un error al recibir los datos",
                        type = "error"
                    });
                }
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrio un error al obtener los datos",
                        type = "error"
                    });
                }
                usuario.Eliminado = true;
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
                    mensaje = "Usuario eliminado correctamente",
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
        private static string ObtenerIconRol(int id_rol)
        {
            Dictionary<int, string> roles = new()
            {
                { 1, "<i class=\"fa-solid fa-user-secret\"></i>" },
                { 2, "<i class=\"fa-solid fa-user-tie\"></i>" },
                { 3, "<i class=\"fa-solid fa-car-side\"></i>" },
                { 4, "<i class=\"fa-solid fa-user-pen\"></i>" },
                { 5, "<i class=\"fa-solid fa-calendar-check\"></i>" },
                { 6, "<i class=\"fa-solid fa-id-card\"></i>" },
                { 7, "<i class=\"fa-solid fa-truck-moving\"></i>" },
                { 8, "<i class=\"fa-solid fa-book\"></i>" }
            };
            if (roles.ContainsKey(id_rol))
                return roles[id_rol];

            return "";
        }

        private static string ObtenerIconPermiso(int id_permiso)
        {
            Dictionary<int, string> iconos = new()
            {
                { 1, "<i class=\"fas fa-file-alt fa-sm\"></i>" },
                { 2, "<i class=\"fas fa-car fa-sm\"></i>" },
                { 3, "<i class=\"fas fa-check-circle fa-sm\"></i>" },
                { 4, "<i class=\"fas fa-chart-bar fa-sm\"></i>" },
                { 5, "<i class=\"fas fa-download fa-sm\"></i>" },
                { 6, "<i class=\"fas fa-list fa-sm\"></i>" },
                { 7, "<i class=\"fas fa-plus-circle fa-sm\"></i>" },
                { 8, "<div class=\"position-relative\">\r\n  <i class=\"fas fa-car fa-sm position-absolute\"></i>\r\n  <i class=\"fas fa-pencil-alt fa-sm position-absolute top-0 start-0\"></i>\r\n</div>" },
                { 9, "<div class=\"position-relative\">\r\n  <i class=\"fas fa-car fa-sm position-absolute\"></i>\r\n  <i class=\"fas fa-trash-alt fa-sm position-absolute top-0 start-0\"></i>\r\n</div>" },
                { 10, "<div class=\"position-relative\">\r\n  <i class=\"fas fa-eye fa-xs position-absolute\"></i>\r\n  <i class=\"fas fa-list fa-sm position-absolute top-0 start-0\"></i>\r\n</div>" },
                { 11, "<i class=\"fas fa-user fa-sm\"></i>" },
                { 12, "<i class=\"fas fa-user-plus fa-sm\"></i>" },
                { 13, "<i class=\"fas fa-user-tag fa-sm\"></i>" }
            };

            if (iconos.ContainsKey(id_permiso))
                return iconos[id_permiso];

            return "";
        }
    }
}
