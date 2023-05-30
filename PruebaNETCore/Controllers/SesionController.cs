using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProyectoSolveCore.Models;
using ProyectoSolveCore.Models.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                //u => u.Id != 1
                return _context.Usuarios.Any() ? View(new vmLoginUser()) : RedirectToAction("CrearUsuarioAdmin");
            }
            return RedirectToAction("Agenda","Home");
        }
        [HttpPost]
        public async Task<IActionResult> Login(vmLoginUser user)
        {
            try 
            {
                var usuario = await _context.Usuarios.Include(u => u.IdDepartamentoNavigation).Include(u => u.UsuariosRoles)
                            .FirstOrDefaultAsync(u => u.Rut.Equals(user.Rut) && !u.Eliminado);

                if (usuario == null)
                {
                    return View(user);
                }
                if (!usuario.Login)
                {
                    return View(user);
                }
                if (!Encrypt.VerifyPassword(user.Clave, usuario.Clave))
                {
                    return View(user);
                }
                //Usuario verificado correctamente
                var roles = await _context.UsuariosRoles.Where(ur => ur.IdUsuario == usuario.Id)
                    .Select(ur => ur.IdRolNavigation).Select(r => r.Rol).ToListAsync();
                bool autenticado = await AutenticarUsuario(usuario, roles);

                return RedirectToAction("Agenda", "Home");
            }
            catch (Exception ex)
            {
                if (user == null)
                {
                    return View(new vmLoginUser());
                }
                return View(user);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        public IActionResult CrearUsuarioAdmin()
        {
            ViewBag.id_departamento = new SelectList(_context.Departamentos.ToList(), "Id", "Departamento1");

            return View(new vmUsuarioConductorRoles()
            {
                RolAdministrador = true,
                RolJefe = true,
                RolMantenedorBitacora = true,
                RolMantendorUsuarios = true,
                RolMantenedorVehiculos = true,
                RolSolicitador = true,
                RolMantenedorVehiculosMaq = true,
            });
        }
        [HttpPost]
        public async Task<IActionResult> CrearUsuarioAdmin(vmUsuarioConductorRoles uc)
        {
            if (uc == null)
            {
                return View(new vmUsuarioConductorRoles());
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
                n = await _context.SaveChangesAsync();
                if (n == 0)
                {
                    return View(uc);
                }

                await transaction.CommitAsync();
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                return View(uc);
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

            for (int i = 0; i < 8; i++)
            {
                if (i + 1 != 6)
                {
                    lur.Add(new vmRolCheck { IdRol = i + 1, IdUsuario = uc.ID, check = roles[i + 1] });
                }
            }
            return lur;
        }
    }
}
