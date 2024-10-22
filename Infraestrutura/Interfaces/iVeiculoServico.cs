using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimal_api.Dominio.Entidades;
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace Minimal_api.Infraestrutura.Interfaces
{
    public interface iVeiculoServico
    {
        List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null);
        Veiculo? BuscaPorId(int id);
        void Incluir(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void Apagar(Veiculo veiculo);
    }
}