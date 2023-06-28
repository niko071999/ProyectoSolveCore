using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using System.Net;

/*
 * Program: Clase principal que act�a como punto de entrada de la aplicaci�n
 */

// Creaci�n del objeto builder para la aplicaci�n web
var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de servicios para controladores con vistas
builder.Services.AddControllersWithViews();
// Registro de servicios con alcance de tiempo de vida Scoped
builder.Services.AddScoped<ModelData>();
builder.Services.AddScoped<VerificarSolicitudes>();

// Configuraci�n de controladores y opciones JSON
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.WriteIndented = true;
        });
// Configuraci�n de la autenticaci�n basada en cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Sesion/Login"; // Ruta a la p�gina de inicio de sesi�n
        option.ExpireTimeSpan = TimeSpan.FromHours(12); // Tiempo de expiraci�n de la cookie
        option.AccessDeniedPath = "/Error/Error401"; // Ruta a la p�gina de acceso denegado
    });
//Creaci�n de la aplicaci�n
var app = builder.Build();

// Configuraci�n para el manejo de p�ginas de c�digo de estado
app.UseStatusCodePages(context => {
    var resp = context.HttpContext.Response;
    if (resp.StatusCode == (int)HttpStatusCode.NotFound)
    {// Redirige a la p�gina de error 404
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

// Configuraci�n de middleware y enrutamiento de la aplicaci�n
app.UseStaticFiles(); // Habilita el uso de archivos est�ticos
app.UseRouting(); // Configura el enrutamiento de solicitudes HTTP en la aplicaci�n
app.UseAuthentication(); // Habilita la autenticaci�n en la aplicaci�n
app.UseAuthorization(); // Habilita la autorizaci�n en la aplicaci�n

// Establece una ruta de controlador predeterminada
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sesion}/{action=Login}/{id?}",
    defaults: new {controller = "Sesion", action = "Login"});
app.Run(); // Ejecuta la aplicaci�n