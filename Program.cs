using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minimal_api.Dominio.Servicos;
using Minimal_api.Infraestrutura.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<iAdministradorServico, AdministradorServico>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("mysql");

    options.UseMySql(connectionString, 
        ServerVersion.AutoDetect(connectionString)); // Usa auto-detect para a versão do MySQL
});

var app = builder.Build();

app.MapGet("/", () => "Olá!");

app.MapPost("/login", ([FromBody]LoginDTO loginDTO, iAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
});

app.Run();

