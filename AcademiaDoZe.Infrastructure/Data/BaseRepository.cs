// Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>, IAsyncDisposable
        where TEntity : Entity
    {
        protected readonly string _connectionString;
        protected readonly DatabaseType _databaseType;

        protected BaseRepository(string connectionString, DatabaseType databaseType)
        {
            _connectionString = connectionString
                ?? throw new InfrastructureException("ERRO_STRING_CONEXAO" + nameof(connectionString));

            _databaseType = databaseType;
        }

        protected virtual string TableName => $"tb_{typeof(TEntity).Name.ToLower()}";
        protected virtual string IdTableName => $"id_{typeof(TEntity).Name.ToLower()}";

        protected virtual async Task<DbConnection> GetOpenConnectionAsync()
        {
            try
            {
                var connection = DbProvider.CreateConnection(_connectionString, _databaseType);
                await connection.OpenAsync();
                return connection;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException("FALHA_ABRIR_CONEXAO", ex);
            }
        }

        public virtual async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            await Task.CompletedTask;
        }

        ~BaseRepository()
        {
            DisposeAsync(false).AsTask().GetAwaiter().GetResult();
        }

        public virtual async Task<TEntity?> ObterPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID_NAO_INFORMADO_MENOR_UM", nameof(id));

            try
            {
                await using var connection = await GetOpenConnectionAsync();

                string query = $"SELECT * FROM {TableName} WHERE {IdTableName} = @Id";

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Id", id, DbType.Int32, _databaseType));

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                    return await MapAsync(reader);

                return null;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"ERRO_OBTER_DADOS_ID_{id}", ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> ObterTodos()
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();

                string query = $"SELECT * FROM {TableName}";

                await using var command = DbProvider.CreateCommand(query, connection);
                await using var reader = await command.ExecuteReaderAsync();

                var entities = new List<TEntity>();

                while (await reader.ReadAsync())
                {
                    entities.Add(await MapAsync(reader));
                }

                return entities;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException("ERRO_OBTER_DADOS_TODOS", ex);
            }
        }

        public virtual async Task<bool> Remover(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID_NAO_INFORMADO_MENOR_UM", nameof(id));

            try
            {
                await using var connection = await GetOpenConnectionAsync();

                string query = $"DELETE FROM {TableName} WHERE {IdTableName} = @Id";

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Id", id, DbType.Int32, _databaseType));

                var result = await command.ExecuteNonQueryAsync();

                return result > 0;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"ERRO_REMOVER_ID_{id}", ex);
            }
        }

        public abstract Task<TEntity> Adicionar(TEntity entity);
        public abstract Task<TEntity> Atualizar(TEntity entity);
        protected abstract Task<TEntity> MapAsync(DbDataReader reader);
    }
}
