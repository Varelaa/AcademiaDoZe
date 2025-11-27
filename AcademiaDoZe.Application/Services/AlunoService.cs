// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Application.Security;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly Func<IAlunoRepository> _repoFactory;
        private readonly Func<IMatriculaRepository> _matriculaRepoFactory;

        public AlunoService(
            Func<IAlunoRepository> repoFactory,
            Func<IMatriculaRepository> matriculaRepoFactory)
        {
            _repoFactory = repoFactory;
            _matriculaRepoFactory = matriculaRepoFactory;
        }

        public async Task<AlunoDTO> ObterPorIdAsync(int id)
        {
            var aluno = await _repoFactory().ObterPorId(id);
            return aluno?.ToDto() ?? throw new KeyNotFoundException("Aluno não encontrado.");
        }

        public async Task<IEnumerable<AlunoDTO>> ObterTodosAsync()
        {
            var alunos = await _repoFactory().ObterTodos();
            return alunos.Select(a => a.ToDto());
        }

        public async Task<AlunoDTO> AdicionarAsync(AlunoDTO dto)
        {
            if (await _repoFactory().CpfJaExisteAsync(dto.Cpf))
                throw new InvalidOperationException($"CPF {dto.Cpf} já cadastrado.");

            if (!string.IsNullOrWhiteSpace(dto.Senha))
                dto.Senha = PasswordHasher.Hash(dto.Senha);

            var aluno = dto.ToEntity();
            await _repoFactory().Adicionar(aluno);

            return aluno.ToDto();
        }

        public async Task<AlunoDTO> AtualizarAsync(AlunoDTO dto)
        {
            var alunoExistente = await _repoFactory().ObterPorId(dto.Id)
                ?? throw new KeyNotFoundException("Aluno não encontrado.");

            if (await _repoFactory().CpfJaExisteAsync(dto.Cpf, dto.Id))
                throw new InvalidOperationException($"CPF {dto.Cpf} já está em uso.");

            // Senha somente se enviada
            if (!string.IsNullOrWhiteSpace(dto.Senha))
                dto.Senha = PasswordHasher.Hash(dto.Senha);
            else
                dto.Senha = alunoExistente.Senha;

            var atualizado = alunoExistente.UpdateFromDto(dto);
            await _repoFactory().Atualizar(atualizado);

            return atualizado.ToDto();
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var aluno = await _repoFactory().ObterPorId(id);
            if (aluno == null) return false;

            await _repoFactory().Remover(id);
            return true;
        }

        public async Task<AlunoDTO> ObterPorCpfAsync(string cpf)
        {
            cpf = new string(cpf.Where(char.IsDigit).ToArray());
            var aluno = await _repoFactory().ObterPorCpfAsync(cpf);
            return aluno?.ToDto() ?? null!;
        }

        public Task<bool> CpfJaExisteAsync(string cpf, int? id = null)
        {
            return _repoFactory().CpfJaExisteAsync(cpf, id);
        }

        public async Task<bool> TrocarSenhaAsync(int id, string novaSenha)
        {
            if (string.IsNullOrWhiteSpace(novaSenha))
                throw new ArgumentException("Senha inválida.");

            var hash = PasswordHasher.Hash(novaSenha);
            return await _repoFactory().TrocarSenhaAsync(id, hash);
        }
    }
}