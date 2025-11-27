// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly Func<IMatriculaRepository> _matriculaRepoFactory;
        private readonly Func<IAlunoRepository> _alunoRepoFactory;

        public MatriculaService(
            Func<IMatriculaRepository> matriculaRepoFactory,
            Func<IAlunoRepository> alunoRepoFactory)
        {
            _matriculaRepoFactory = matriculaRepoFactory
                ?? throw new ArgumentNullException(nameof(matriculaRepoFactory));

            _alunoRepoFactory = alunoRepoFactory
                ?? throw new ArgumentNullException(nameof(alunoRepoFactory));
        }

        public async Task<MatriculaDTO?> ObterPorIdAsync(int id)
        {
            var matricula = await _matriculaRepoFactory().ObterPorId(id);
            return matricula?.ToDto();
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterTodasAsync()
        {
            var list = await _matriculaRepoFactory().ObterTodos();
            return list.Select(m => m.ToDto()).ToList();
        }

        public async Task<MatriculaDTO> AdicionarAsync(MatriculaDTO matriculaDto)
        {
            var aluno = await _alunoRepoFactory().ObterPorId(matriculaDto.AlunoId)
                ?? throw new KeyNotFoundException($"Aluno ID {matriculaDto.AlunoId} não encontrado.");

            var matriculasAtivas = await ObterMatriculasAtivasAluno(matriculaDto.AlunoId);
            if (matriculasAtivas.Any())
                throw new InvalidOperationException("O aluno já possui matrícula ativa.");

            var matricula = matriculaDto.ToEntity(aluno);

            await _matriculaRepoFactory().Adicionar(matricula);

            return matricula.ToDto();
        }

        public async Task<MatriculaDTO> AtualizarAsync(MatriculaDTO matriculaDto)
        {
            var repo = _matriculaRepoFactory();

            var existente = await repo.ObterPorId(matriculaDto.Id)
                ?? throw new KeyNotFoundException($"Matrícula ID {matriculaDto.Id} não encontrada.");

            var atualizada = existente.UpdateFromDto(matriculaDto);

            await repo.Atualizar(atualizada);

            return atualizada.ToDto();
        }
        public async Task<bool> RemoverAsync(int id)
        {
            var repo = _matriculaRepoFactory();

            var existente = await repo.ObterPorId(id);
            if (existente == null) return false;

            await repo.Remover(id);
            return true;
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterPorAlunoIdAsync(int alunoId)
        {
            var todas = await _matriculaRepoFactory().ObterTodos();

            return todas
                .Where(m => m.AlunoMatricula.Id == alunoId)
                .Select(m => m.ToDto())
                .ToList();
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterAtivasAsync(int alunoId = 0)
        {
            var repo = _matriculaRepoFactory();
            var todas = await repo.ObterTodos();

            var hoje = DateOnly.FromDateTime(DateTime.Now);

            var query = (alunoId > 0)
                ? todas.Where(m => m.AlunoMatricula.Id == alunoId)
                : todas;

            return query
                .Where(m => m.DataInicio <= hoje && m.DataFim >= hoje)
                .Select(m => m.ToDto())
                .ToList();
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterVencendoEmDiasAsync(int dias)
        {
            var repo = _matriculaRepoFactory();

            var hoje = DateOnly.FromDateTime(DateTime.Now);
            var limite = DateOnly.FromDateTime(DateTime.Now.AddDays(dias));

            var todas = await repo.ObterTodos();

            return todas
                .Where(m => m.DataFim >= hoje && m.DataFim <= limite)
                .Select(m => m.ToDto())
                .ToList();
        }

        public async Task<TimeSpan> ObterTempoRestantePlanoAsync(int alunoId)
        {
            var ativas = await ObterMatriculasAtivasAluno(alunoId);
            var ativa = ativas.FirstOrDefault();
            if (ativa == null) return TimeSpan.Zero;

            var diasRestantes =
                ativa.DataFim.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber;

            return TimeSpan.FromDays(Math.Max(0, diasRestantes));
        }

        private async Task<IEnumerable<MatriculaDTO>> ObterMatriculasAtivasAluno(int alunoId)
        {
            var repo = _matriculaRepoFactory();

            var todas = await repo.ObterTodos();
            var hoje = DateOnly.FromDateTime(DateTime.Now);

            return todas
                .Where(m => m.AlunoMatricula.Id == alunoId &&
                            m.DataInicio <= hoje &&
                            m.DataFim >= hoje)
                .Select(m => m.ToDto())
                .ToList();
        }
    }
}
