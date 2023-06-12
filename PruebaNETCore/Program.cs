using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});
builder.Services.AddScoped<ModelData>();
builder.Services.AddScoped<VerificarSolicitudes>();

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.WriteIndented = true;
        });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Sesion/Login";
        option.ExpireTimeSpan = TimeSpan.FromHours(12);
        option.AccessDeniedPath = "/Error/Error401";
    });
var app = builder.Build();

app.UseStatusCodePages(context => {
    var resp = context.HttpContext.Response;
    if (resp.StatusCode == (int)HttpStatusCode.NotFound)
    {
        resp.Redirect("/Error/Error404");
    }
    return Task.CompletedTask;
});
// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sesion}/{action=Login}/{id?}",
    defaults: new {controller = "Sesion", action = "Login"});
app.Run();