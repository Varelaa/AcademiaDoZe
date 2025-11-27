//Gabriel Souza Varela

using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.DTOs
{
    public class MatriculaDTO
    {
        public int Id { get; set; }

        public int AlunoId { get; set; }   
        public AlunoDTO? Aluno { get; set; } 

        public required EMatriculaPlano Plano { get; set; }
        public required DateOnly DataInicio { get; set; }
        public required DateOnly DataFim { get; set; }
        public required string Objetivo { get; set; }
        public required EMatriculaRestricoes RestricoesMedicas { get; set; }
        public string? ObservacoesRestricoes { get; set; }
        public ArquivoDTO? LaudoMedico { get; set; }
    }
}
