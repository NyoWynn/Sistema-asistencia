using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;

var builder = WebApplication.CreateBuilder(args);

// --- INICIO DE LA CONFIGURACIÓN IMPORTANTE ---

// 1. Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. AÑADIR Y CONFIGURAR EL DBCONTEXT.
//    Esta es la línea que resuelve el error. Le "enseña" a la aplicación
//    cómo crear el ApplicationDbContext.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- FIN DE LA CONFIGURACIÓN IMPORTANTE ---

// Añadir servicios para controladores y vistas
builder.Services.AddControllersWithViews();

// Añadir soporte para Sesiones (lo usaremos para el Login)
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

// Cambiar la ruta inicial a nuestra página de Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();