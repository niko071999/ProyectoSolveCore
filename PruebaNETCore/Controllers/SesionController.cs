using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ProyectoSolveCore.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con la sesión de los usuarios.
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona acciones para ver, crear y modificar 
    /// las sesiones de los usuarios en el sistema.
    /// </remarks>
    public class SesionController : Controller
    {
        /// <summary>
        /// Contexto de la base de datos
        /// </summary>
        private readonly ModelData _context;
        /// <summary>
        /// Registro de eventos y mensajes de registro.
        /// </summary>
        private readonly ILogger<SesionController> _ilogger;
        //Constructor
        public SesionController(ModelData context, ILogger<SesionController> ilogger)
        {
            _context = context;
            _ilogger = ilogger;
        }
        /// <summary>
        /// Muestra el formulario de acceso al sistema
        /// </summary>
        /// <returns>Vista con el formulario para ingresar al sistema</returns>
        public IActionResult Login()
        {
            //Verificar si el usuario esta autenticado
            if (!User.Identity.IsAuthenticated)
            {//Si no esta, enviar un mensaje de cierra de sesión 
                if (ViewBag.MensajeLogout != null)
                {
                    TempData["MensajeLogout"] = ViewBag.MensajeLogout;
                }
            }
            else
            {
                return View(new vmLoginUser());
            }
            //Redireccionar a la vista Agenda si el usuario esta autenticado
            return RedirectToAction("Agenda","Home");
        }

        private static void EnviarCorreo()
        {
            //MailMessage correo = new("nikocasanova10@gmail.com","nicolas.casanova06@inacapmail.cl")
            //{
            //    Subject = "Correo de prueba",
            //    Body = "Este es un correo de prueba enviado desde C# ASP.NET CORE",
            //    IsBodyHtml = true,
            //};
            //correo.To.Add("nicolas.casanova06@inacapmail.cl"); //Correo destino?

            //SmtpClient smtp = new()
            //{
            //    Host = "smtp.gmail.com", //Host del servidor de correo,
            //    Port = 587, //Puerto de salida,
            //    UseDefaultCredentials = false,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    EnableSsl = true,
            //    Credentials = new NetworkCredential("nikocasanova10@gmail.com", "**********")//Cuenta de correo
            //};
            //smtp.Send(correo);
        }
        /// <summary>
        /// Se realiza la petición de autenticación
        /// </summary>
        /// <param name="user">Los datos de acceso del usuario</param>
        /// <returns>La vista correspondiente según el resultado de la autenticación del usuario</returns>
        [HttpPost]
        public async Task<IActionResult> Login(vmLoginUser user)
        {
            string mensaje = "Los datos de acceso ingresados no son correctos";
            if (string.IsNullOrEmpty(user.Rut.Trim()) || string.IsNullOrEmpty(user.Clave.Trim()))
            {
                TempData["Mensaje"] = mensaje;
                return View(new vmLoginUser());
            }
            try 
            {
                var usuario = await _context.Usuarios.Include(u => u.IdDepartamentoNavigation).Include(u => u.Usuariosroles)
                            .Where(u => u.Rut.Equals(user.Rut) && !u.Eliminado).FirstOrDefaultAsync();
                //Se verifica si el usuario no existe
                if (usuario == null)
                {
                    TempData["Mensaje"] = mensaje;
                    return View(user);
                }
                //Se verifica si el usuario no tiene permisos de acceso al sistema
                if (!usuario.Login)
                {
                    TempData["Mensaje"] = "El usuario no tiene permisos para acceder al sistema. Contacte con el administrador del sistema o inténtelo nuevamente.";
                    return View(user);
                }
                //Se verifican las claves
                if (!Encrypt.VerifyPassword(user.Clave, usuario.Clave))
                {
                    TempData["Mensaje"] = mensaje;
                    return View(user);
                }
                //Usuario verificado correctamente

                var roles = await _context.Usuariosroles.Where(ur => ur.Idusuario == usuario.Id)
                    .Select(ur => ur.IdrolNavigation).Select(r => r.Rol).ToListAsync();
                bool autenticado = await AutenticarUsuario(usuario, roles);
                if (!autenticado)
                {
                    TempData["Mensaje"] = "Hubo un error al intentar autenticar el usuario en el sistema. Contacte con el administrador del sistema o inténtelo nuevamente.";
                    return View(user);
                }
                //Si el usuario es autenticado exitosamente, se redirecciona a la vista de la agenda general.
                return RedirectToAction("Agenda", "Home");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Ocurrió un error inesperado en el sistema. Póngase en contacto con el administrador o inténtelo mas tarde.";
                if (user == null)
                {
                    return View(new vmLoginUser());
                }
                return View(user);
            }
        }
        /// <summary>
        /// Cierra la sesión del usuario
        /// </summary>
        /// <returns>Redirecciona al usuario a la vista de inicio de sesión</returns>
        public async Task<IActionResult> Logout()
        {
            //Cierre de sesión del usuario utilizando el esquema de autenticación de cookies por defecto.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.MensajeLogout = "Se a cerrado sesión correctamente";
            return RedirectToAction("Login");
        }
        /// <summary>
        /// Autentica un usuario y establece las reclamaciones (claims) de identidad para el usuario autenticado.
        /// </summary>
        /// <param name="usuario">Usuario a autenticar.</param>
        /// <param name="roles">Lista de roles asociados al usuario.</param>
        /// <returns>True si la autenticación es exitosa; de lo contrario sera False.</returns>
        private async Task<bool> AutenticarUsuario(Usuario usuario, List<string> roles)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim("Id", usuario.Id.ToString()),
                    new Claim("Rut", usuario.Rut),
                    new Claim("Departamento",usuario.IdDepartamentoNavigation.Departamento1),
                    new Claim(ClaimTypes.Name, usuario.Nombre +" "+usuario.Apellido),
                    new Claim("Imagen", usuario.DireccionImg)
                };
                foreach (string rol in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol));
                }
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
