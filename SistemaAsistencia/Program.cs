using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;

var builder = WebApplication.CreateBuilder(args);

// --- INICIO DE LA CONFIGURACI�N IMPORTANTE ---

// 1. Obtener la cadena de conexi�n desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. A�ADIR Y CONFIGURAR EL DBCONTEXT.
//    Esta es la l�nea que resuelve el error. Le "ense�a" a la aplicaci�n
//    c�mo crear el ApplicationDbContext.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- FIN DE LA CONFIGURACI�N IMPORTANTE ---

// A�adir servicios para controladores y vistas
builder.Services.AddControllersWithViews();

// A�adir soporte para Sesiones (lo usaremos para el Login)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Habilitar el uso de Sesiones
app.UseSession();

// Cambiar la ruta inicial a nuestra p�gina de Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();