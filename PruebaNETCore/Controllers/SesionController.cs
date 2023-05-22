using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoSolveCore.Controllers
{
    public class SesionController : Controller
    {
        private readonly ModelData _context;

        public SesionController(ModelData context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return _context.Usuarios.Any() ? View(new vmLoginUser()): RedirectToAction("CrearUsuarioAdmin");
            }
            return RedirectToAction("Agenda","Home");
        }
        [HttpPost]
        public async Task<IActionResult> Login(vmLoginUser user)
        {
            if (!_context.Usuarios.Any())
            {
                return RedirectToAction("CrearUsuarioAdmin", "Usuario", user.Rut);
            }
            var usuario = _context.Usuarios.Include(u => u.IdDepartamentoNavigation).Include(u => u.UsuariosRoles).FirstOrDefault(u => u.Rut.Equals(user.Rut));
            if (usuario == null)
            {
                return View(user);
            }
            if (!Encrypt.VerifyPassword(user.Clave,usuario.Clave))
            {
                return View(user);
            }
            //Usuario verificado correctamente
            var roles = _context.UsuariosRoles.Where(ur => ur.IdUsuario == usuario.Id)
                .Select(ur => ur.IdRolNavigation).Select(r => r.Rol).ToList();
            bool autenticado = await AutenticarUsuario(usuario, roles);

            return RedirectToAction("Agenda","Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        public IActionResult CrearUsuarioAdmin(string rut)
        {
            var usuario = new Usuario
            {
                Rut = rut
            };
            return View(usuario);
        }
        [HttpPost]
        public async Task<IActionResult> CrearUsuarioAdmin(Usuario usuario)
        {
            if (usuario == null || usuario.Clave.Trim().Length < 6)
            {
                return View(new Usuario());
            }
            var userAny = await _context.Usuarios.AnyAsync(u => u.Rut == usuario.Rut);
            if (userAny)
            {
                return View(usuario);
            }
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                var u = new Usuario()
                {
                    Rut = usuario.Rut,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Clave = Encrypt.EncryptPassword(usuario.Clave),
                    IdDepartamento = usuario.IdDepartamento,
                    DireccionImg = "",
                    Eliminado = false
                };

                _context.Usuarios.Add(u);
                int n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    return View(usuario);
                }
                var UsuarioRoles = _context.Roles.Select(r => new UsuariosRole()
                {
                    IdRol = r.Id,
                    IdUsuario = u.Id
                });
                await _context.UsuariosRoles.AddRangeAsync(UsuarioRoles);
                n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    return View(usuario);
                }
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Login));
            }
            catch (Exception)
            {
                return View(usuario);
            }
        }
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
