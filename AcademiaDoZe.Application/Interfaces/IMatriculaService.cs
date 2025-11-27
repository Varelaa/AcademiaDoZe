// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces
{
    public interface IMatriculaService
    {
        Task<MatriculaDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<MatriculaDTO>> ObterTodasAsync();
        Task<IEnumerable<MatriculaDTO>> ObterPorAlunoIdAsync(int alunoId);
        Task<IEnumerable<MatriculaDTO>> ObterAtivasAsync(int alunoId = 0);
        Task<IEnumerable<MatriculaDTO>> ObterVencendoEmDiasAsync(int dias);

        Task<MatriculaDTO> AdicionarAsync(MatriculaDTO matriculaDto);
        Task<MatriculaDTO> AtualizarAsync(MatriculaDTO matriculaDto);

        Task<bool> RemoverAsync(int id);

        Task<TimeSpan> ObterTempoRestantePlanoAsync(int alunoId);
    }
}
