using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using System.Net;

/*
 * Program: Clase principal que actúa como punto de entrada de la aplicación
 */

// Creación del objeto builder para la aplicación web
var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios para controladores con vistas
builder.Services.AddControllersWithViews();
// Registro de servicios con alcance de tiempo de vida Scoped
builder.Services.AddScoped<ModelData>();
builder.Services.AddScoped<VerificarSolicitudes>();

// Configuración de controladores y opciones JSON
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.WriteIndented = true;
        });
// Configuración de la autenticación basada en cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Sesion/Login"; // Ruta a la página de inicio de sesión
        option.ExpireTimeSpan = TimeSpan.FromHours(12); // Tiempo de expiración de la cookie
        option.AccessDeniedPath = "/Error/Error401"; // Ruta a la página de acceso denegado
    });
//Creación de la aplicación
var app = builder.Build();

// Configuración para el manejo de páginas de código de estado
app.UseStatusCodePages(context => {
    var resp = context.HttpContext.Response;
    if (resp.StatusCode == (int)HttpStatusCode.NotFound)
    {// Redirige a la página de error 404
        resp.Redirect("/Error/Error404");
    }
    return Task.CompletedTask;
});

// Comentar al publicar en el servidor NGNIX
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}
//app.UseHttpsRedirection();

// Configuración de middleware y enrutamiento de la aplicación
app.UseStaticFiles(); // Habilita el uso de archivos estáticos
app.UseRouting(); // Configura el enrutamiento de solicitudes HTTP en la aplicación
app.UseAuthentication(); // Habilita la autenticación en la aplicación
app.UseAuthorization(); // Habilita la autorización en la aplicación

// Establece una ruta de controlador predeterminada
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sesion}/{action=Login}/{id?}",
    defaults: new {controller = "Sesion", action = "Login"});
app.Run(); // Ejecuta la aplicación