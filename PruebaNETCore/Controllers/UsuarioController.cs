﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con los usuarios.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver, crear, modificar y eliminar 
    /// los usuarios del sistema.
    /// </remarks>
    [Authorize]
    public class UsuarioController : Controller
    {
        /// <summary>
        /// Contexto de la base de datos
        /// </summary>
        private readonly ModelData _context;
        //Constructor
        public UsuarioController(ModelData context)
        {
            _context = context;
        }
        /// <summary>
        /// Método para visualizar todos los usuarios creados.
        /// </summary>
        /// <returns>Vista que muestra la lista de usuarios creados.</returns>
        [Autorizar(permisoId:10)]
        public IActionResult VisualizarUsuarios()
        {
            try
            {
                var usuarios = _context.Usuarios.Where(u => !u.Eliminado
                    && u.Id != 1
                    && u.Id != 2
                    && u.Id != int.Parse(User.FindFirst("Id").Value))
                .Select(u => new vmUsuario()
                {
                    Id = u.Id,
                    Rut = u.Rut,
                    Nombre = u.Nombre + " " + u.Apellido,
                    Departamento = u.IdDepartamentoNavigation.Departamento1,
                    NombreVehiculo = u.Conductores.SelectMany(c => c.Vehiculos).FirstOrDefault(),
                    Roles = u.Usuariosroles.Select(ur => ur.IdrolNavigation).Select(r => new vmRol()
                    {
                        Id = r.Id,
                        rol = r.Rol,
                        IconRol = ObtenerIconRol(r.Id)
                    }).ToList(),
                    Permisos = u.Usuariosroles.Select(ur => ur.IdrolNavigation).SelectMany(r => r.RolesPermisos)
                    .Select(rp => rp.IdPermisoNavigation)
                    .Select(p => new VmPermiso()
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
                return View(new vmUsuario());
            }
        }
        /// <summary>
        /// Método que muestra formulario para ingresar un nuevo usuario.
        /// </summary>
        /// <returns>Vista que muestra formulario para crear nuevo usuario.</returns>
        [Autorizar(permisoId:12)]
        public async Task<IActionResult> AgregarUsuario()
        {
            ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", null, "Group"); 
            return View(new vmUsuarioConductorRoles());
        }
        /// <summary>
        /// Agrega un nuevo usuario.
        /// </summary>
        /// <param name="uc">Contiene los datos del usuario, roles y/o conductor.</param>
        /// <returns>Vista que representa el resultado de la acción.</returns>
        [Autorizar(permisoId: 12)]
        [HttpPost]
        public async Task<IActionResult> AgregarUsuario(vmUsuarioConductorRoles uc)
        {
            //Se verifica que el objeto no venga vació
            if (uc == null)
            {
                return View(new vmUsuarioConductorRoles());
            }
            //Consulta si existe el RUT en el sistema ya ingresado
            var userAny = await _context.Usuarios.Where(u => !u.Eliminado).AnyAsync(u => u.Rut == uc.rut);
            if (userAny)//ERROR: Verifica si existe otro usuario con el mismo RUT
            {
                ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", uc.id_vehiculo, "Group");
                return View(uc);
            }
            if (uc.login && string.IsNullOrEmpty(uc.clave.Trim()))
            {//Error: Si el usuario tiene permisos de acceder al sistema, pero la contraseña esta vacía
                ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", uc.id_vehiculo, "Group");
                return View(uc);
            }
            try
            {
                //Comienza la transacción a la base de datos
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
                    ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", uc.id_vehiculo, "Group");
                    return View(uc);
                }
                uc.ID = usuario.Id;
                var UsuarioRole = ObtenerRoles(uc).Where(ur => ur.check).Select(ur => new Usuariosrole()
                {
                    Idrol = ur.IdRol,
                    Idusuario = ur.IdUsuario
                });

                await _context.Usuariosroles.AddRangeAsync(UsuarioRole);
                await _context.SaveChangesAsync();

                // VERIFICA SI EXISTEN DATOS DEL CONDUCTOR, SI ESTÁN NULOS GUARDA EN BASE DE DATOS LOS DATOS DE USUARIO
                if (uc.FechaEmitida == null || uc.FecheVencimiento == null || uc.NumeroPoliza == null)
                {
                    if (n == 0)
                    {
                        ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", uc.id_vehiculo, "Group");
                        return View(uc);
                    }

                    await transaction.CommitAsync();
                    return RedirectToAction(nameof(VisualizarUsuarios));
                }
                //Se agrega el rol de conductor
                await _context.Usuariosroles.AddAsync(new Usuariosrole()
                {
                    Idrol = 6,
                    Idusuario = usuario.Id
                });
                await _context.SaveChangesAsync();

                var conductor = new Conductor()
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
                    ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", uc.id_vehiculo, "Group");
                    return View(uc);
                }

                if (uc.id_vehiculo.HasValue)
                {
                    var vehiculo = await _context.Vehiculos.FindAsync(uc.id_vehiculo);
                    if (vehiculo == null) //VERIFICA SI EL ELEMENTO ES VACIO, SI ES ASI, DIRIGE A LA VISTA
                    {
                        ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", uc.id_vehiculo, "Group");
                        return View(uc);
                    }
                    vehiculo.IdConductor = conductor.Id;
                    n = await _context.SaveChangesAsync();
                    if (n == 0)
                    {
                        ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", uc.id_vehiculo, "Group");
                        return View(uc);
                    }
                }
                //Se termina la transacción y se realiza commit
                await transaction.CommitAsync();
                return RedirectToAction("VisualizarUsuarios");
            }
            catch (Exception ex)
            {
                ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", uc.id_vehiculo, "Group");
                return View(uc);
            }
        }
        /// <summary>
        /// Método que muestra formulario para modificar el perfil de un usuario.
        /// </summary>
        /// <returns>Vista que muestra formulario para modificar el perfil de un usuario.</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Perfil()
        {
            try
            {
                int id = int.Parse(User.FindFirst("Id").Value);
                var perfil = await _context.Usuarios.Select(u => new vmUsuarioConductorRoles
                {
                    ID = u.Id,
                    rut = u.Rut,
                    rutold = u.Rut,//Sirve para saber si cambio el RUT
                    nombre = u.Nombre,
                    apellido = u.Apellido,
                    direccion_img = u.DireccionImg,
                    FechaEmitida = u.Conductores.Where(u => u.Id == id).FirstOrDefault().FechaEmision,
                    FecheVencimiento = u.Conductores.Where(u => u.Id == id).FirstOrDefault().FechaVencimiento,
                    id_conductor = u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).Id,
                    id_departamento = u.IdDepartamento,
                    NumeroPoliza = u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).NumeroPoliza,
                    id_vehiculo = u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).Vehiculos.FirstOrDefault().Id,
                    RolAdministrador = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 1),
                    RolJefe = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 2),
                    RolMantenedorVehiculos = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 3),
                    RolMantendorUsuarios = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 4),
                    RolSolicitador = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 5),
                    RolMantenedorVehiculosMaq = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 7),
                    RolMantenedorBitacora = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 8),
                }).Where(u => u.ID == id).FirstOrDefaultAsync();
                if (perfil == null)
                {
                    return View(new Usuario());
                }
                //Se configura los SelectList para mostrarlo en el elemento SELECT HTML
                var departamentos = await _context.Departamentos.ToListAsync();
                ViewBag.id_departamento = new SelectList(departamentos, "Id", "Departamento1", perfil.id_departamento);
                ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", null, "Group");
                return View(perfil);
            }
            catch (Exception ex)
            {
                return View(new Usuario());
            }
        }
        /// <summary>
        /// Acción para editar el perfil del usuario.
        /// </summary>
        /// <param name="perfil">Contiene los datos del perfil a editar.</param>
        /// <returns>Vista que representa el resultado de la acción.</returns>
        [HttpPost]
        public async Task<IActionResult> EditarPerfil(vmUsuarioConductorRoles perfil)
        {
            //Se verifica si el objeto viene vació
            if (perfil == null)
            {
                TempData["Mensaje"] = "Ocurrió un error al recibir los datos";
                return RedirectToAction("Perfil");
            }
            try
            {
                //Obtengo el resultado del método y se lo paso al objeto de la clase JsonMensaje
                JsonMensaje jm = await ModificarUsuario(perfil);
                if (jm.Type.Equals("success"))
                {
                    TempData["MensajeSuccess"] = jm.Mensaje;
                    return RedirectToAction("Perfil");
                }
                TempData["Mensaje"] = jm.Mensaje;
                return View(perfil);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Ocurrió un error inesperado, avise al administrador o inténtelo más tarde";
                return RedirectToAction("Perfil");
            }
        }
        /// <summary>
        /// Acción para mostrar el formulario para editar un usuario.
        /// </summary>
        /// <param name="id">ID del usuario a editar.</param>
        /// <returns>Vista que muestra el formulario para editar un usuario.</returns>
        [Autorizar(permisoId: 17)]
        public async Task<PartialViewResult> EditarUsuario(int id = 0)
        {
            try
            {
                if (id == 0)
                {//Si el ID es vació, devolver una ventana emergente con un mensaje de error.
                    string mensaje = "Hubo un error al recibir los datos";
                    return PartialView("_PartialModalError", mensaje);
                }
                //Se obtiene el usuario con el ID recibido
                var usuario = await _context.Usuarios
                    .Where(u => !u.Eliminado && u.Id == id).Select(u => new vmUsuarioConductorRoles()
                {
                    ID = u.Id,
                    rutold = u.Rut,//Sirve para verificar si el usuario modifico el RUT
                    rut = u.Rut,
                    nombre = u.Nombre,
                    apellido = u.Apellido,
                    login = u.Login,
                    id_departamento = u.IdDepartamentoNavigation.Id,
                    direccion_img = u.DireccionImg,
                    id_conductor = u.Conductores.Any() ? u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).Id:null,
                    id_vehiculo = u.Conductores.Select(c => c.Vehiculos).Any() ? 
                        u.Conductores.SelectMany(c => c.Vehiculos).FirstOrDefault().Id : null,
                    NumeroPoliza = u.Conductores.Any() ? u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).NumeroPoliza : null,
                    FechaEmitida = u.Conductores.Any() ? u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).FechaEmision : null,
                    FecheVencimiento = u.Conductores.Any() ? u.Conductores.FirstOrDefault(c => c.IdUsuario == u.Id).FechaVencimiento : null,
                    RolAdministrador = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 1),
                    RolJefe = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 2),
                    RolMantenedorVehiculos = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 3),
                    RolMantendorUsuarios = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 4),
                    RolSolicitador = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 5),
                    RolMantenedorVehiculosMaq = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 7),
                    RolMantenedorBitacora = u.Usuariosroles.Any(ur => ur.Idusuario == u.Id && ur.Idrol == 8)
                }).FirstOrDefaultAsync();
                if (usuario == null)
                {//Si el usuario no existe, mostrar una ventana emergente con un mensaje de error
                    return PartialView("_PartialModalError", usuario);
                }
                //Configurar los SelectList para mostrarlo en el elemento SELECT HTML
                var departamentos = await _context.Departamentos.ToListAsync();
                ViewBag.id_departamento = new SelectList(departamentos, "Id", "Departamento1", usuario.id_departamento);
                ViewBag.id_vehiculo = new SelectList(await GetVehiculosWithCategoria(), "Value", "Text", usuario.id_vehiculo.HasValue
                           ? usuario.id_vehiculo : null, "Group");
                return PartialView("_EditarUsuario", usuario);
            }
            catch (Exception ex)
            {
                string mensaje = "Hubo un error inesperado";
                return PartialView("_PartialModalError",mensaje);
            }
        }
        /// <summary>
        /// Método que edita un usuario.
        /// </summary>
        /// <param name="uc">Contiene los datos del usuario a editar.</param>
        /// <returns>Un json que contiene el resultado de la acción.</returns>
        [Autorizar(permisoId: 17)]
        [HttpPost]
        public async Task<JsonResult> EditarUsuario(vmUsuarioConductorRoles uc)
        {
            if (uc == null)
            {
                return Json(new
                {
                    mensaje = "Ocurrió un error al recibir los datos",
                    type = "error"
                });
            }
            try
            {
                //Obtengo el resultado del método y se lo paso al objeto de la clase JsonMensaje
                JsonMensaje jm = await ModificarUsuario(uc);
                return Json(new
                {
                    mensaje = jm.Mensaje,
                    type = jm.Type
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    mensaje = "Ocurrió un error inesperado, avise al administrador o inténtelo más tarde",
                    type = "error"
                });
            }
        }
        /// <summary>
        /// Acción para mostrar el formulario para eliminar un usuario.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar.</param>
        /// <returns>Vista que muestra el formulario para eliminar un usuario.</returns>
        [Autorizar(permisoId: 18)]
        public async Task<PartialViewResult> EliminarUsuario(int id = 0)
        {
            string mensaje;
            if (id == 0)
            {
                mensaje = "Hubo un error al recibir los datos, inténtelo nuevamente";
                return PartialView("_PartialModalError", mensaje);
            }
            try
            {
                if (id == 0)
                {
                    mensaje = "Hubo un error al recibir los datos, inténtelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }
                //Se busca el usuario con el id recibido del usuario para pasarlo a la vista
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    mensaje = "Hubo un error al recibir los datos, inténtelo nuevamente";
                    return PartialView("_PartialModalError", mensaje);
                }

                return PartialView("_BorrarUsuario", usuario);
            }
            catch (Exception)
            {
                string m = "Ocurrió un error inesperado, consulte con administrador del sistema o inténtelo nuevamente";
                return PartialView("_PartialModalError", m);
            }
        }
        /// <summary>
        /// Acción para eliminar un usuario del sistema.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar.</param>
        /// <returns>Un json que contiene el resultado de la acción.</returns>
        [Autorizar(permisoId: 18)]
        [HttpPost]
        public async Task<JsonResult> BorrarUsuario(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrió un error al recibir los datos",
                        type = "error"
                    });
                }
                var usuario = await _context.Usuarios.Include(u => u.Conductores).Where(u => u.Id == id).FirstOrDefaultAsync();
                if (usuario == null)
                {
                    return Json(new
                    {
                        mensaje = "Ocurrió un error al obtener los datos",
                        type = "error"
                    });
                }
                usuario.Conductores.FirstOrDefault().Eliminado = true;
                usuario.Eliminado = true;
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
                    mensaje = "Usuario eliminado correctamente",
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
        /// Método que se utiliza para agregar un nuevo departamento 
        /// desde los formularios de agregar o editar usuario.
        /// </summary>
        /// <param name="departamento">Nombre del departamento.</param>
        /// <returns>Un json que contiene un mensaje y el departamento creado.</returns>
        public async Task<JsonResult> SelectAgregarDepartamento(string departamento)
        {
            if (string.IsNullOrEmpty(departamento.Trim()))
            {
                return Json(new
                {
                    data = "",
                    type = "void",
                    mensaje = "El nombre departamento está vació"
                });
            }
            //Se recupera los departamentos y se verifica si ya existe en el sistema
            var departamentos = await _context.Departamentos.ToListAsync();
            if (departamentos.Any(x => x.Departamento1.Equals(departamento, StringComparison.OrdinalIgnoreCase)))
            {//Si existe, enviar un error
                return Json(new
                {
                    data = "",
                    type = "error",
                    mensaje = "Ocurrió un error al verificar los departamentos, ya existe un registro con este nombre"
                });
            }

            var d = new Departamento()
            {
                Departamento1 = departamento,
            };
            await _context.Departamentos.AddAsync(d);
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
                data = d,
                type = "success",
                mensaje = "Se a agregado el departamento exitosamente"
            });
        }
        /// <summary>
        /// Método que se utiliza para conseguir el vehículo asignado al conductor
        /// </summary>
        /// <param name="id">ID del conductor.</param>
        /// <returns>Un json que contiene el id del vehículo.</returns>
        public async Task<JsonResult> GetVehiculoConductor(int id = 0)
        {
            int idvehiculo = 0;
            if (id == 0)
            {
                return Json(idvehiculo);
            }
            var conductor = await _context.Conductores.Where(v => v.Id == id).FirstOrDefaultAsync();
            if (conductor != null)
            {
                var vehiculo = await _context.Vehiculos.Where(v => v.IdConductor.HasValue && v.IdConductor == id).FirstOrDefaultAsync();
                if (vehiculo != null)
                {
                    idvehiculo = vehiculo.Id;
                    return Json(idvehiculo);
                }
            }
            return Json(idvehiculo);
        }
        /// <summary>
        /// Método privado para modificar un usuario en la base de datos.
        /// </summary>
        /// <param name="uc">Contiene los datos del usuario a modificar.</param>
        /// <returns>Un json que indica el resultado de la modificación.</returns>
        private async Task<JsonMensaje> ModificarUsuario(vmUsuarioConductorRoles uc)
        {
            try
            {
                var u = await _context.Usuarios.Where(u => !u.Eliminado && u.Rut == uc.rut).FirstOrDefaultAsync();
                if (u != null)
                {
                    if (uc.rut != uc.rutold)
                    {
                        //Verifico si hay mas de 1 elemento con el RUT del usuario
                        return new JsonMensaje()
                        {
                            Mensaje = "El RUT pertenece a otro usuario registrado",
                            Type = "error"
                        };
                    }
                }

                //Comienzo de la transacción a la base de datos
                using var transaction = await _context.Database.BeginTransactionAsync();

                var usuario = await _context.Usuarios.FindAsync(uc.ID);
                usuario.Rut = uc.rut;
                usuario.Nombre = uc.nombre;
                usuario.Apellido = uc.apellido;
                usuario.Login = uc.login;
                if (!string.IsNullOrEmpty(uc.clave))
                {
                    usuario.Clave = Encrypt.EncryptPassword(uc.clave);
                }
                usuario.IdDepartamento = uc.id_departamento;
                usuario.DireccionImg = uc.direccion_img;
                usuario.Eliminado = false;
                _context.Entry(usuario).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var UsuarioRole = ObtenerRoles(uc);
                var RolesUsuario = await _context.Usuariosroles.Where(ur => ur.Idusuario == uc.ID).ToListAsync();
                int contains = 0;//Cuenta si hubieron roles que se ingresaron a la base de datos para verificar
                foreach (var ur in UsuarioRole)
                {
                    var usuarioRol = await _context.Usuariosroles.Where(x =>
                        x.Idrol == UsuarioRole[contains].IdRol
                        && x.Idusuario == UsuarioRole[contains].IdUsuario).FirstOrDefaultAsync();

                    if (usuarioRol != null)
                    {
                        if (!UsuarioRole[contains].check)
                        {
                            _context.Usuariosroles.Remove(usuarioRol); // Eliminar el registro
                            await _context.SaveChangesAsync();
                        }
                        // No hacer nada si el rol existe y está seleccionado
                    }
                    else if (UsuarioRole[contains].check)
                    {
                        var nuevoUsuarioRol = new Usuariosrole()
                        {
                            Idusuario = UsuarioRole[contains].IdUsuario,
                            Idrol = UsuarioRole[contains].IdRol
                        };
                        await _context.Usuariosroles.AddAsync(nuevoUsuarioRol); // Agregar el nuevo registro
                        await _context.SaveChangesAsync();
                    }
                    contains++;
                }

                // VERIFICA SI EXISTEN DATOS DEL CONDUCTOR, SI ESTÁN NULOS GUARDA EN BASE DE DATOS LOS DATOS DE USUARIO
                var rolConductor = await _context.Usuariosroles.Where(ur =>
                    ur.Idrol == 6
                    && ur.Idusuario == uc.ID).FirstOrDefaultAsync();
                if (uc.FechaEmitida == null || uc.FecheVencimiento == null || uc.NumeroPoliza == null)
                {
                    //Quitar el id del conductor del vehículo asignado
                    var vehiculo = await _context.Vehiculos.Where(v => v.Id == uc.ID).FirstOrDefaultAsync();
                    if (vehiculo.IdConductor.HasValue)
                    {
                        vehiculo.IdConductor = null;
                        _context.Vehiculos.Update(vehiculo);
                        await _context.SaveChangesAsync();
                    }

                    //Borrar rol conductor
                    if (rolConductor != null)
                    {
                        _context.Remove(rolConductor);
                        await _context.SaveChangesAsync();
                    }
                    //Borrar conductor
                    var c = _context.Conductores.Find(uc.id_conductor);
                    if (c != null)
                    {
                        _context.Conductores.Remove(c);
                        await _context.SaveChangesAsync();
                    }

                    if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new JsonMensaje()
                    {
                        Mensaje = "Usuario editado correctamente",
                        Type = "success"
                    };
                }

                if (rolConductor == null)
                {
                    await _context.Usuariosroles.AddAsync(new Usuariosrole()
                    {
                        Idrol = 6,
                        Idusuario = uc.ID,
                    });
                }

                //Actualizar el conductor
                var conductor = await _context.Conductores.FindAsync(uc.id_conductor);
                if (conductor != null)
                {
                    conductor.Estado = true;
                    conductor.FechaEmision = (DateTime)uc.FechaEmitida;
                    conductor.FechaVencimiento = (DateTime)uc.FecheVencimiento;
                    conductor.IdUsuario = usuario.Id;
                    conductor.NumeroPoliza = (int)uc.NumeroPoliza;
                    _context.Conductores.Update(conductor);
                }
                else
                {
                    conductor = new Conductor()
                    {
                        Estado = true,
                        FechaEmision = (DateTime)uc.FechaEmitida,
                        FechaVencimiento = (DateTime)uc.FecheVencimiento,
                        IdUsuario = usuario.Id,
                        NumeroPoliza = (int)uc.NumeroPoliza,
                    };
                    await _context.Conductores.AddAsync(conductor);
                }

                if (uc.id_vehiculo.HasValue)
                {
                    var vehiculo = await _context.Vehiculos.Where(v => v.Id == uc.id_vehiculo).FirstOrDefaultAsync();
                    if (uc.id_conductor != vehiculo.IdConductor)
                    {
                        vehiculo.IdConductor = conductor.Id;
                        _context.Update(vehiculo);
                    }
                }
                if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new JsonMensaje()
                {
                    Mensaje = "Usuario editado correctamente",
                    Type = "success"
                };
            }
            catch (Exception ex)
            {
                return new JsonMensaje()
                {
                    Mensaje = "Ocurrió un error inesperado, avise al administrador del sistema o intente mas tarde",
                    Type = "error"
                };
            }
        }
        /// <summary>
        /// Método para obtener una lista de roles del usuario.
        /// </summary>
        /// <param name="uc">Contiene los datos del usuario.</param>
        /// <returns>Lista de roles con los roles seleccionados del usuario.</returns>
        private static List<vmRolCheck> ObtenerRoles(vmUsuarioConductorRoles uc)
        {
            //lur = Lista Usuario Roles
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

            for (int i = 0; i < 8; i++)
            {
                if (i + 1 != 6)
                {
                    lur.Add(new vmRolCheck { IdRol = i + 1, IdUsuario = uc.ID, check = roles[i + 1] });
                }
            }
            return lur;
        }
        /// <summary>
        /// Método para obtener una lista de vehículos con categoría.
        /// </summary>
        /// <returns>Lista de vehículos con sus categorías.</returns>
        public async Task<List<SelectListWithGroups>> GetVehiculosWithCategoria()
        {
            return await _context.Vehiculos.Include(v => v.IdCategoriaNavigation)
                    .Where(v => !v.Eliminado)
                    .Select(v => new SelectListWithGroups()
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Patente} - {v.Marca} {v.Modelo}",
                        Group = v.IdCategoriaNavigation.Categoria1
                    }).ToListAsync();
        }
        /// <summary>
        /// Método para obtener una lista de iconos de roles del usuario.
        /// </summary>
        /// <returns>Lista de iconos de roles del usuario.</returns>
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
        /// <summary>
        /// Método para obtener una lista de iconos de permisos del usuario.
        /// </summary>
        /// <returns>Lista de iconos de permisos del usuario.</returns>
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
            {
                return iconos[id_permiso];
            }
            return "";
        }
    }
}
