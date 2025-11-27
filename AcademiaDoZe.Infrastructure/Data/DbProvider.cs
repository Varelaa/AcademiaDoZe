// Gabriel Souza Varela

using AcademiaDoZe.Infrastructure.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace AcademiaDoZe.Infrastructure.Data
{
    public enum DatabaseType
    {
        SqlServer,
        MySql
    }

    public static class DbProvider
    {
        public const int DefaultCommandTimeout = 30;

        public static DbConnection CreateConnection(string connectionString, DatabaseType dbType)
        {
            try
            {
                DbConnection connection = dbType switch
                {
                   // DatabaseType.SqlServer => new SqlConnection(connectionString),
                    DatabaseType.MySql => new MySqlConnection(connectionString),
                    _ => throw new InfrastructureException($"SGDB_NAO_SUPORTADO {dbType}")
                };

                return connection ?? throw new InfrastructureException($"FALHA_CONEXAO {dbType}");
            }
            catch (DbException ex)
            {
                throw new InfrastructureException($"FALHA_CONEXAO {dbType}", ex);
            }
        }

        public static DbCommand CreateCommand(string commandText, DbConnection connection, CommandType commandType = CommandType.Text)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection), "COMMAND_CONEXAO_NULL");

            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentException("COMMAND_TEXT_VAZIO", nameof(commandText));

            try
            {
                var command = connection.CreateCommand() ?? throw new InfrastructureException("FALHA_CRIAR_COMANDO");
                command.CommandText = commandText;
                command.CommandType = commandType;
                command.CommandTimeout = DefaultCommandTimeout;
                return command;
            }
            catch (DbException ex)
            {
                throw new InfrastructureException("FALHA_CRIAR_COMANDO", ex);
            }
        }

        public static DbParameter CreateParameter(string name, object value, DbType dbType, DatabaseType databaseType)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("PARAMETER_VAZIO", nameof(name));

            try
            {
                DbParameter parameter = databaseType switch
                {
                    //DatabaseType.SqlServer => new SqlParameter(),
                    DatabaseType.MySql => new MySqlParameter(),
                    _ => throw new InfrastructureException($"SGDB_NAO_SUPORTADO {databaseType}")
                };

                parameter.ParameterName = name;
                parameter.Value = value ?? DBNull.Value;
                parameter.DbType = dbType;

                return parameter;
            }
            catch (DbException ex)
            {
                throw new InfrastructureException("ERRO_CRIAR_PARAMETRO", ex);
            }
        }
    }
}
