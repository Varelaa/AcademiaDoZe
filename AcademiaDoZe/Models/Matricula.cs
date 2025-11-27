//Gabriel Souza Varela

using System.Collections.Generic;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Models
{
    public class Matricula
    {
        public Aluno Aluno { get; set; }
        public EMatriculaPlano Plano { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Objetivo { get; set; }
        public List<string> Restricoes { get; set; }
        public string Observacoes { get; set; }
        public string LaudoMedico { get; set; }

        public Matricula(
            Aluno aluno,
            EMatriculaPlano plano,
            DateTime dataInicio,
            DateTime dataFim,
            string objetivo,
            List<string> restricoes,
            string observacoes,
            string laudoMedico = null)
        {
            Aluno = aluno;
            Plano = plano;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Objetivo = objetivo;
            Restricoes = restricoes;
            Observacoes = observacoes;
            LaudoMedico = laudoMedico;
        }
    }
}
