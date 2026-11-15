using Microsoft.EntityFrameworkCore;
using TiendaEcommerce.Api.Data;
using TiendaEcommerce.Api.Repositories;
using TiendaEcommerce.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Conexión a PostgreSQL mediante Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Inyección de dependencias por capas (Usuarios y Catálogo)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();

// 3. Política CORS para permitir peticiones desde el frontend en React + TypeScript
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("PermitirFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();