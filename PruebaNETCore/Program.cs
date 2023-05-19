using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using ProyectoSolveCore.Filters;
using ProyectoSolveCore.Models;
using PruebaNETCore.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
//option =>
//{
//    option.Filters.Add(typeof(AccessDeniedFilter));
//}
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ModelData>();
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
        option.ExpireTimeSpan = TimeSpan.FromHours(24);
        option.AccessDeniedPath = "/Sesion/AccessDenied";
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sesion}/{action=Login}/{id?}",
    defaults: new {controller = "Sesion", action = "Login"});

app.Run();
