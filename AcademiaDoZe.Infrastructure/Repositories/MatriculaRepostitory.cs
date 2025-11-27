//Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class MatriculaRepository : BaseRepository<Matricula>, IMatriculaRepository
    {
        public MatriculaRepository(string connectionString, DatabaseType databaseType)
            : base(connectionString, databaseType) { }

        public override async Task<Matricula> Adicionar(Matricula entity)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = _databaseType == DatabaseType.SqlServer
                ? $"INSERT INTO {TableName} (aluno_id, plano, inicio, fim, objetivo, restricoes, observacoes) " +
                  "OUTPUT INSERTED.id_matricula VALUES (@AlunoId, @Plano, @Inicio, @Fim, @Objetivo, @Restricoes, @Observacoes);"
                : $"INSERT INTO {TableName} (aluno_id, plano, inicio, fim, objetivo, restricoes, observacoes) " +
                  "VALUES (@AlunoId, @Plano, @Inicio, @Fim, @Objetivo, @Restricoes, @Observacoes); SELECT LAST_INSERT_ID();";

            await using var command = DbProvider.CreateCommand(query, connection);

            command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", entity.AlunoMatricula.Id, DbType.Int32, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Plano", (int)entity.Plano, DbType.Int32, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Inicio", entity.DataInicio, DbType.Date, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Fim", entity.DataFim, DbType.Date, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Objetivo", entity.Objetivo, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Restricoes", (int)entity.RestricoesMedicas, DbType.Int32, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Observacoes", entity.ObservacoesRestricoes ?? "", DbType.String, _databaseType));

            var id = await command.ExecuteScalarAsync();
            typeof(Entity).GetProperty("Id")!.SetValue(entity, Convert.ToInt32(id));

            return entity;
        }

        public override async Task<Matricula> Atualizar(Matricula entity)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query =
                $"UPDATE {TableName} SET plano=@Plano, inicio=@Inicio, fim=@Fim, objetivo=@Objetivo, restricoes=@Restricoes, observacoes=@Observacoes " +
                $"WHERE id_matricula=@Id";

            await using var command = DbProvider.CreateCommand(query, connection);

            command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Plano", (int)entity.Plano, DbType.Int32, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Inicio", entity.DataInicio, DbType.Date, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Fim", entity.DataFim, DbType.Date, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Objetivo", entity.Objetivo, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Restricoes", (int)entity.RestricoesMedicas, DbType.Int32, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Observacoes", entity.ObservacoesRestricoes ?? "", DbType.String, _databaseType));

            await command.ExecuteNonQueryAsync();
            return entity;
        }

        public async Task<IEnumerable<Matricula>> ObterPorAlunoAsync(int alunoId)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = $"SELECT * FROM {TableName} WHERE aluno_id = @AlunoId";

            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", alunoId, DbType.Int32, _databaseType));

            using var reader = await command.ExecuteReaderAsync();

            var lista = new List<Matricula>();
            while (await reader.ReadAsync())
                lista.Add(await MapAsync(reader));

            return lista;
        }

        public async Task<IEnumerable<Matricula>> ObterPorAlunoIdAsync(int alunoId)
        {
            return await ObterPorAlunoAsync(alunoId);
        }

        public async Task<IEnumerable<Matricula>> ObterAtivasAsync(int alunoId)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = $"SELECT * FROM {TableName} WHERE aluno_id = @AlunoId AND fim >= CURRENT_DATE";

            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", alunoId, DbType.Int32, _databaseType));

            using var reader = await command.ExecuteReaderAsync();

            var lista = new List<Matricula>();
            while (await reader.ReadAsync())
                lista.Add(await MapAsync(reader));

            return lista;
        }

        public async Task<IEnumerable<Matricula>> ObterVencendoEmDiasAsync(int dias)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = _databaseType == DatabaseType.SqlServer
                ? $"SELECT * FROM {TableName} WHERE fim <= DATEADD(DAY, @Dias, GETDATE())"
                : $"SELECT * FROM {TableName} WHERE fim <= DATE_ADD(CURDATE(), INTERVAL @Dias DAY)";

            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@Dias", dias, DbType.Int32, _databaseType));

            using var reader = await command.ExecuteReaderAsync();

            var lista = new List<Matricula>();
            while (await reader.ReadAsync())
                lista.Add(await MapAsync(reader));

            return lista;
        }

        protected override async Task<Matricula> MapAsync(DbDataReader reader)
        {
            var aluno = Aluno.Criar(
                reader["nome"].ToString()!,
                reader["cpf"].ToString()!,
                DateOnly.FromDateTime(Convert.ToDateTime(reader["nascimento"]))!,
                reader["telefone"].ToString()!,
                reader["email"].ToString()!,
                Logradouro.Criar("00000000", "Rua", "Bairro", "Cidade", "ST", "Brasil"),
                "0",
                "",
                reader["senha"].ToString()!,
                null
            );

            typeof(Entity).GetProperty("Id")!.SetValue(aluno, Convert.ToInt32(reader["aluno_id"]));

            var matricula = Matricula.Criar(
                aluno,
                (EMatriculaPlano)Convert.ToInt32(reader["plano"]),
                DateOnly.FromDateTime(Convert.ToDateTime(reader["inicio"])),
                DateOnly.FromDateTime(Convert.ToDateTime(reader["fim"])),
                reader["objetivo"].ToString()!,
                (EMatriculaRestricoes)Convert.ToInt32(reader["restricoes"]),
                null,
                reader["observacoes"].ToString()!
            );

            typeof(Entity).GetProperty("Id")!.SetValue(matricula, Convert.ToInt32(reader["id_matricula"]));
            return matricula;
        }
    }
}