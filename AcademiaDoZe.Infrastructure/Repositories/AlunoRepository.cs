using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class AlunoRepository : BaseRepository<Aluno>, IAlunoRepository
    {
        public AlunoRepository(string connectionString, DatabaseType databaseType)
            : base(connectionString, databaseType) { }

        public override async Task<Aluno> Adicionar(Aluno entity)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = _databaseType == DatabaseType.SqlServer
                ? $"INSERT INTO {TableName} (cpf, nome, nascimento, email, telefone, numero, complemento, senha, logradouro_id) " +
                  "OUTPUT INSERTED.id_aluno VALUES (@Cpf, @Nome, @Nascimento, @Email, @Telefone, @Numero, @Complemento, @Senha, @LogradouroId);"
                : $"INSERT INTO {TableName} (cpf, nome, nascimento, email, telefone, numero, complemento, senha, logradouro_id) " +
                  "VALUES (@Cpf, @Nome, @Nascimento, @Email, @Telefone, @Numero, @Complemento, @Senha, @LogradouroId); SELECT LAST_INSERT_ID();";

            await using var command = DbProvider.CreateCommand(query, connection);

            command.Parameters.Add(DbProvider.CreateParameter("@Cpf", entity.Cpf, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Nome", entity.Nome, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Nascimento", entity.DataNascimento, DbType.Date, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Email", entity.Email, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Telefone", entity.Telefone, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Numero", entity.Numero, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Complemento", entity.Complemento, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Senha", entity.Senha, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@LogradouroId", entity.Endereco.Id, DbType.Int32, _databaseType));

            var id = await command.ExecuteScalarAsync();

            if (id != null && id != DBNull.Value)
            {
                typeof(Entity).GetProperty("Id")!.SetValue(entity, Convert.ToInt32(id));
            }

            return entity;
        }

        public override async Task<Aluno> Atualizar(Aluno entity)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query =
                $"UPDATE {TableName} SET cpf=@Cpf, nome=@Nome, nascimento=@Nascimento, email=@Email, telefone=@Telefone, " +
                $"numero=@Numero, complemento=@Complemento, senha=@Senha, logradouro_id=@LogradouroId WHERE id_aluno=@Id";

            await using var command = DbProvider.CreateCommand(query, connection);

            command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Cpf", entity.Cpf, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Nome", entity.Nome, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Nascimento", entity.DataNascimento, DbType.Date, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Email", entity.Email, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Telefone", entity.Telefone, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Numero", entity.Numero, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Complemento", entity.Complemento, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Senha", entity.Senha, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@LogradouroId", entity.Endereco.Id, DbType.Int32, _databaseType));

            int rows = await command.ExecuteNonQueryAsync();

            if (rows == 0)
                throw new InvalidOperationException($"ALUNO_NAO_ENCONTRADO_ID_{entity.Id}");

            return entity;
        }

        public async Task<Aluno?> ObterPorCpfAsync(string cpf)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = $"SELECT a.*, l.* FROM {TableName} a " +
                           $"INNER JOIN tb_logradouro l ON l.id_logradouro = a.logradouro_id " +
                           $"WHERE a.cpf = @Cpf";

            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@Cpf", cpf, DbType.String, _databaseType));

            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? await MapAsync(reader) : null;
        }

        public async Task<bool> CpfJaExisteAsync(string cpf, int? id = null)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = $"SELECT COUNT(1) FROM {TableName} WHERE cpf = @Cpf";
            if (id.HasValue) query += " AND id_aluno != @Id";

            await using var command = DbProvider.CreateCommand(query, connection);

            command.Parameters.Add(DbProvider.CreateParameter("@Cpf", cpf, DbType.String, _databaseType));

            if (id.HasValue)
                command.Parameters.Add(DbProvider.CreateParameter("@Id", id.Value, DbType.Int32, _databaseType));

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }

        public async Task<bool> TrocarSenhaAsync(int id, string novaSenha)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = $"UPDATE {TableName} SET senha = @Senha WHERE id_aluno = @Id";

            await using var command = DbProvider.CreateCommand(query, connection);

            command.Parameters.Add(DbProvider.CreateParameter("@Id", id, DbType.Int32, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Senha", novaSenha, DbType.String, _databaseType));

            var rows = await command.ExecuteNonQueryAsync();

            return rows > 0;
        }

        protected override async Task<Aluno> MapAsync(DbDataReader reader)
        {
            var logradouro = Logradouro.Criar(
                reader["cep"].ToString()!,
                reader["logradouro"].ToString()!,
                reader["bairro"].ToString()!,
                reader["cidade"].ToString()!,
                reader["estado"].ToString()!,
                reader["pais"].ToString()!
            );

            typeof(Entity).GetProperty("Id")!.SetValue(logradouro, Convert.ToInt32(reader["id_logradouro"]));

            var aluno = Aluno.Criar(
                reader["nome"].ToString()!,
                reader["cpf"].ToString()!,
                DateOnly.FromDateTime(Convert.ToDateTime(reader["nascimento"])),
                reader["telefone"].ToString()!,
                reader["email"].ToString()!,
                logradouro,
                reader["numero"].ToString()!,
                reader["complemento"].ToString()!,
                reader["senha"].ToString()!,
                null
            );

            typeof(Entity).GetProperty("Id")!.SetValue(aluno, Convert.ToInt32(reader["id_aluno"]));

            return aluno;
        }

        public async Task<IEnumerable<Matricula>> ObterMatriculasAsync(int alunoId)
        {
            throw new NotImplementedException();
        }
    }
}
