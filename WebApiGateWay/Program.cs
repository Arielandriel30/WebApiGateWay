using Microsoft.EntityFrameworkCore;
using WebApiGateWay.Entidades.Context;
using WebApiGateWay.Services;

var builder = WebApplication.CreateBuilder(args);
// Configurar DbContext con la cadena de conexión
builder.Services.AddDbContext<UltimaMilla2Context>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 4, 32))));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Servicios e Interfaces
builder.Services.AddSingleton<IAuthorizationMiddlewareService, AuthorizationMiddlewareService>();
builder.Services.AddSingleton<IJwtService, JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
