// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.Mappings
{
    public static class ColaboradorMappings
    {
        public static ColaboradorDTO ToDto(this Colaborador colaborador)
        {
            return new ColaboradorDTO
            {
                Id = colaborador.Id,
                Nome = colaborador.Nome,
                Cpf = colaborador.Cpf,
                DataNascimento = colaborador.DataNascimento,
                Telefone = colaborador.Telefone,
                Email = colaborador.Email,
                Endereco = colaborador.Endereco.ToDto(),
                Numero = colaborador.Numero,
                Complemento = colaborador.Complemento,
                Senha = null,

                Foto = colaborador.Foto is not null
                    ? new ArquivoDTO { Conteudo = colaborador.Foto.Conteudo }
                    : null,

                DataAdmissao = colaborador.DataAdmissao,
                Tipo = colaborador.Tipo,
                Vinculo = colaborador.Vinculo
            };
        }

        public static Colaborador ToEntity(this ColaboradorDTO dto)
        {
            return Colaborador.Criar(
                dto.Id,
                dto.Nome,
                dto.Cpf,
                dto.DataNascimento,
                dto.Telefone,
                dto.Email ?? string.Empty,
                dto.Endereco.ToEntity(),
                dto.Numero,
                dto.Complemento ?? string.Empty,
                dto.Senha ?? string.Empty,
                dto.Foto?.Conteudo != null ? Arquivo.Criar(dto.Foto.Conteudo) : null,
                dto.DataAdmissao,
                dto.Tipo,
                dto.Vinculo
            );
        }

        public static Colaborador UpdateFromDto(this Colaborador colaborador, ColaboradorDTO dto)
        {
            return Colaborador.Criar(
                colaborador.Id,
                dto.Nome ?? colaborador.Nome,
                colaborador.Cpf,
                dto.DataNascimento != default ? dto.DataNascimento : colaborador.DataNascimento,
                dto.Telefone ?? colaborador.Telefone,
                dto.Email ?? colaborador.Email,
                dto.Endereco?.ToEntity() ?? colaborador.Endereco,
                dto.Numero ?? colaborador.Numero,
                dto.Complemento ?? colaborador.Complemento,
                dto.Senha ?? colaborador.Senha,
                dto.Foto?.Conteudo != null ? Arquivo.Criar(dto.Foto.Conteudo) : colaborador.Foto,
                dto.DataAdmissao != default ? dto.DataAdmissao : colaborador.DataAdmissao,
                dto.Tipo != 0 ? dto.Tipo : colaborador.Tipo,
                dto.Vinculo != 0 ? dto.Vinculo : colaborador.Vinculo
            );
        }
    }
}
