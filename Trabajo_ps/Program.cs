using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore; //para manejar sql desde c#

// inicia el constructor del servidor web. Es la base de toda la app.
var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor - get, post, urls, swagger.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Inyección de Dependencias
builder.Services.AddApplication(); //carga servicios con application
builder.Services.AddInfrastructure(builder.Configuration); //lee las pass/links de bd

// CORS para el frontend - permite que navegadores consulten la api
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        //permite peticiones de web externa cualquiera 
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

//construye la app
var app = builder.Build();

// Crear base de datos y cargar datos de prueba "espacio temporal" 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

// Tubería de peticiones http
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<Trabajo_ps.Middlewares.ExceptionMiddleware>();

app.UseDefaultFiles(); //index
app.UseStaticFiles(); //app.js , styles.css o img
app.UseCors("AllowAll"); //activa el guardia de frontera creado 
app.UseAuthorization(); //agrega capa seguridad
app.MapControllers(); //conecta rutas web conc controllers en c#

//prende server y espera peticiones
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.Run();
