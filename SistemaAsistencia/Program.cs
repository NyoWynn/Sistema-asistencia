using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));


builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
    
        context.Database.Migrate();
        
       
        await CreateDefaultAdminUser(context);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();


app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();


static async Task CreateDefaultAdminUser(ApplicationDbContext context)
{
    // Verificar si ya existe un usuario administrador
    var adminExists = await context.Users.AnyAsync(u => u.IsAdmin);
    
    if (!adminExists)
    {
        var defaultAdmin = new User
        {
            Name = "Administrador",
            Email = "admin@sistema.com",
            Password = "admin123", 
            IsAdmin = true
        };
        
        context.Users.Add(defaultAdmin);
        await context.SaveChangesAsync();
        
        Console.WriteLine("✅ Usuario administrador creado:");
        Console.WriteLine("   📧 Email: admin@sistema.com");
        Console.WriteLine("   🔑 Contraseña: admin123");
        Console.WriteLine("   ⚠️  IMPORTANTE: Cambia la contraseña después del primer login");
    }
}