// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings
{
    public static class MatriculaMappings
    {
        public static MatriculaDTO ToDto(this Matricula matricula)
        {
            return new MatriculaDTO
            {
                Id = matricula.Id,

                AlunoId = matricula.AlunoMatricula.Id,
                Aluno = matricula.AlunoMatricula.ToDto(),

                Plano = matricula.Plano,
                DataInicio = matricula.DataInicio,
                DataFim = matricula.DataFim,
                Objetivo = matricula.Objetivo,

                RestricoesMedicas = matricula.RestricoesMedicas,
                ObservacoesRestricoes = matricula.ObservacoesRestricoes,

                LaudoMedico = matricula.LaudoMedico != null
                    ? new ArquivoDTO { Conteudo = matricula.LaudoMedico.Conteudo }
                    : null
            };
        }

        public static Matricula ToEntity(this MatriculaDTO dto, Aluno aluno)
        {
            return Matricula.Criar(
                dto.Id,
                aluno,
                dto.Plano,
                dto.DataInicio,
                dto.DataFim,
                dto.Objetivo,
                dto.RestricoesMedicas,
                dto.LaudoMedico?.Conteudo != null
                    ? Arquivo.Criar(dto.LaudoMedico.Conteudo)
                    : null,
                dto.ObservacoesRestricoes ?? ""
            );
        }
        public static Matricula UpdateFromDto(this Matricula matricula, MatriculaDTO dto)
        {
            return Matricula.Criar(
                matricula.Id,
                matricula.AlunoMatricula,

                dto.Plano != default ? dto.Plano : matricula.Plano,
                dto.DataInicio != default ? dto.DataInicio : matricula.DataInicio,
                dto.DataFim != default ? dto.DataFim : matricula.DataFim,
                dto.Objetivo ?? matricula.Objetivo,

                dto.RestricoesMedicas != default
                    ? dto.RestricoesMedicas
                    : matricula.RestricoesMedicas,

                dto.LaudoMedico?.Conteudo != null
                    ? Arquivo.Criar(dto.LaudoMedico.Conteudo)
                    : matricula.LaudoMedico,

                dto.ObservacoesRestricoes ?? matricula.ObservacoesRestricoes
            );
        }
    }
}
