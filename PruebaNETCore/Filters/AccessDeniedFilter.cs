using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ProyectoSolveCore.Filters
{
    public class AccessDeniedFilter : IAuthorizationFilter
    {
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public AccessDeniedFilter(ITempDataDictionaryFactory tempDataFactory)
        {
            _tempDataFactory = tempDataFactory;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Verificar si la solicitud ya está en la página de inicio de sesión
                var loginPath = context.HttpContext.Request.Path.ToString();
                if (!string.Equals(loginPath, "/Sesion/Login", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(loginPath, "/", StringComparison.OrdinalIgnoreCase))
                {
                    var tempData = _tempDataFactory.GetTempData(context.HttpContext);
                    tempData["ErrorMessage"] = "Debe iniciar sesión para acceder al modulo.";
                                    
                    context.Result = new RedirectToActionResult("Login", "Sesion", null);
                }
            }
        }
    }
}
