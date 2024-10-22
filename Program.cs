using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minimal_api.Dominio.DTO;
using Minimal_api.Dominio.Entidades;
using Minimal_api.Dominio.Enuns;
using Minimal_api.Dominio.ModelViews;
using Minimal_api.Dominio.Servicos;
using Minimal_api.Infraestrutura.Interfaces;
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<iAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<iVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("mysql");

    options.UseMySql(connectionString, 
        ServerVersion.AutoDetect(connectionString)); // Usa auto-detect para a versão do MySQL
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody]LoginDTO loginDTO, iAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
}).WithTags("Administradores");

app.MapPost("/administradores", ([FromBody]AdministradorDTO administradorDTO, iAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };
    if(string.IsNullOrEmpty(administradorDTO.Email)){
        validacao.Mensagens.Add("Email não pode ser vazio!");
    }
    if(string.IsNullOrEmpty(administradorDTO.Senha)){
        validacao.Mensagens.Add("Senha não pode ser vazia!");
    }
    if(administradorDTO.Perfil == null){
        validacao.Mensagens.Add("Perfil não pode ser vazio!");
    }
    if(validacao.Mensagens.Count > 0){
        return Results.BadRequest(validacao);
    }
    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };
    administradorServico.Incluir(administrador);
    return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
}).WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery]int? pagina, iAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach(var adm in administradores){
        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms); 
}).WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute]int id, iAdministradorServico administradorServico) => {
    var administrador = administradorServico.BuscaPorId(id);
    if(administrador == null) return Results.NotFound("Veiculo não encontrado!");
    return Results.Ok(new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
}).WithTags("Administradores");
#endregion

#region Veiculos
ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao{
        Mensagens = new List<string>()
    };
    if (string.IsNullOrEmpty(veiculoDTO.Nome))
    {
        validacao.Mensagens.Add("O nome não pode ser vazio!");
    }
        if (string.IsNullOrEmpty(veiculoDTO.Marca))
    {
        validacao.Mensagens.Add("O marca não pode ficar em branco!");
    }
        if (veiculoDTO.Ano < 1950)
    {
        validacao.Mensagens.Add("Veículo muito antigo, favor cadastrar um veiculo com ano superior a 1950.");
    }
    return validacao;
}
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, iVeiculoServico veiculoServico) =>
{
    var validacao = validaDTO(veiculoDTO);
        if (validacao.Mensagens.Count > 0){
        return Results.BadRequest(validacao);
    }
    var veiculo = new Veiculo {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery]int? pagina, iVeiculoServico veiculoServico) => {
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute]int id, iVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound("Veiculo não encontrado!");
    return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute]int id, VeiculoDTO veiculoDto ,iVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound("Veiculo não encontrado!");

    var validacao = validaDTO(veiculoDto);
        if (validacao.Mensagens.Count > 0){
        return Results.BadRequest(validacao);
    }

    veiculo.Nome = veiculoDto.Nome;
    veiculo.Marca = veiculoDto.Marca;
    veiculo.Ano = veiculoDto.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute]int id,iVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound("Veiculo não encontrado!");

    veiculoServico.Apagar(veiculo);
    
    return Results.NoContent();
}).WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
