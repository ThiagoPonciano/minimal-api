using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimal_api.Dominio.DTO;
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace Minimal_api.Infraestrutura.Interfaces
{
    public interface iAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);
        Administrador Incluir(Administrador administrador);
        List<Administrador> Todos(int? pagina);
        Administrador? BuscaPorId(int id);
    }
}