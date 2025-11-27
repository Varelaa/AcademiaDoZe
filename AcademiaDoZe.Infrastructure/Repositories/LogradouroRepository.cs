//Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class LogradouroRepository : BaseRepository<Logradouro>, ILogradouroRepository
    {
        public LogradouroRepository(string connectionString, DatabaseType databaseType)
            : base(connectionString, databaseType) { }

        public override async Task<Logradouro> Adicionar(Logradouro entity)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = _databaseType == DatabaseType.SqlServer
                ? $"INSERT INTO {TableName} (cep, nome, bairro, cidade, estado, pais) OUTPUT INSERTED.id_logradouro " +
                  "VALUES (@Cep, @Nome, @Bairro, @Cidade, @Estado, @Pais);"
                : $"INSERT INTO {TableName} (cep, nome, bairro, cidade, estado, pais) " +
                  "VALUES (@Cep, @Nome, @Bairro, @Cidade, @Estado, @Pais); SELECT LAST_INSERT_ID();";

            await using var command = DbProvider.CreateCommand(query, connection);

            command.Parameters.Add(DbProvider.CreateParameter("@Cep", entity.Cep, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Nome", entity.Nome, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Bairro", entity.Bairro, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Cidade", entity.Cidade, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Estado", entity.Estado, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Pais", entity.Pais, DbType.String, _databaseType));

            var id = await command.ExecuteScalarAsync();
            typeof(Entity).GetProperty("Id")!.SetValue(entity, Convert.ToInt32(id));

            return entity;
        }

        public override async Task<Logradouro> Atualizar(Logradouro entity)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query =
                $"UPDATE {TableName} SET cep=@Cep, nome=@Nome, bairro=@Bairro, cidade=@Cidade, estado=@Estado, pais=@Pais WHERE id_logradouro=@Id";

            await using var command = DbProvider.CreateCommand(query, connection);

            command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Cep", entity.Cep, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Nome", entity.Nome, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Bairro", entity.Bairro, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Cidade", entity.Cidade, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Estado", entity.Estado, DbType.String, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@Pais", entity.Pais, DbType.String, _databaseType));

            await command.ExecuteNonQueryAsync();
            return entity;
        }

        public async Task<Logradouro?> ObterPorCepAsync(string cep)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = $"SELECT * FROM {TableName} WHERE cep = @Cep";

            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@Cep", cep, DbType.String, _databaseType));

            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? await MapAsync(reader) : null;
        }

        public async Task<IEnumerable<Logradouro>> ObterPorCidadeAsync(string cidade)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = $"SELECT * FROM {TableName} WHERE cidade = @Cidade";

            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@Cidade", cidade, DbType.String, _databaseType));

            using var reader = await command.ExecuteReaderAsync();

            var lista = new List<Logradouro>();
            while (await reader.ReadAsync())
                lista.Add(await MapAsync(reader));

            return lista;
        }

        protected override async Task<Logradouro> MapAsync(DbDataReader reader)
        {
            var logradouro = Logradouro.Criar(
                reader["cep"].ToString()!,
                reader["nome"].ToString()!,
                reader["bairro"].ToString()!,
                reader["cidade"].ToString()!,
                reader["estado"].ToString()!,
                reader["pais"].ToString()!
            );

            typeof(Entity).GetProperty("Id")!.SetValue(logradouro, Convert.ToInt32(reader["id_logradouro"]));
            return logradouro;
        }
    }
}
