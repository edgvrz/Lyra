using Lyra.Data;
using Lyra.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Lyra.Services;
using Lyra.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
// Repositorios
builder.Services.AddScoped<IPrendaRepository,   PrendaRepository>();
builder.Services.AddScoped<IFavoritoRepository, FavoritoRepository>();

// Servicios
builder.Services.AddScoped<BodyTypeService>();
builder.Services.AddScoped<RecomendacionService>();
builder.Services.AddScoped<ImagenService>();
builder.Services.AddScoped<ColorimetriaService>();

// Permitir imágenes hasta 10MB en forms
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});
// Sesión para el carrito
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout    = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly  = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name    = "precisa_session";
});
builder.Services.AddScoped<CarritoService>();
var app = builder.Build();


// CREAR ROLES AUTOMÁTICAMENTE
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles =
    {
        UserRoles.Admin,
        UserRoles.Cliente,
        UserRoles.Tienda
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();