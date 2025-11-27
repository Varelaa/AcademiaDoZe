//Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class AcessoRepository : BaseRepository<Acesso>, IAcessoRepository
    {
        public AcessoRepository(string connectionString, DatabaseType databaseType)
            : base(connectionString, databaseType) { }

        private Aluno CriarDummyAluno()
        {
            var log = Logradouro.Criar("00000000", "X", "X", "X", "SP", "BR");

            var aluno = Aluno.Criar(
                "TEMP",
                "00000000000",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-18)),
                "00000000",
                "temp@email.com",
                log,
                "0",
                "",
                "123",
                null
            );

            return aluno;
        }

        private Acesso CriarAcessoDummy(EPessoaTipo tipo, DateTime dataHora)
        {
            var dummy = CriarDummyAluno();
            return Acesso.Criar(tipo, dummy, dataHora);
        }

        public async Task RegistrarEntradaAsync(int pessoaId, DateTime dataHora)
        {
            await using var connection = await GetOpenConnectionAsync();

            string sql = $"INSERT INTO {TableName} (aluno_id, entrada) VALUES (@PessoaId, @DataHora)";

            await using var cmd = DbProvider.CreateCommand(sql, connection);
            cmd.Parameters.Add(DbProvider.CreateParameter("@PessoaId", pessoaId, DbType.Int32, _databaseType));
            cmd.Parameters.Add(DbProvider.CreateParameter("@DataHora", dataHora, DbType.DateTime, _databaseType));

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RegistrarSaidaAsync(int pessoaId, DateTime dataHora)
        {
            await using var connection = await GetOpenConnectionAsync();

            string sql = $"UPDATE {TableName} SET saida = @Saida WHERE aluno_id = @PessoaId AND saida IS NULL";

            await using var cmd = DbProvider.CreateCommand(sql, connection);
            cmd.Parameters.Add(DbProvider.CreateParameter("@PessoaId", pessoaId, DbType.Int32, _databaseType));
            cmd.Parameters.Add(DbProvider.CreateParameter("@Saida", dataHora, DbType.DateTime, _databaseType));

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Acesso>> GetAcessosPorAlunoPeriodoAsync(int? alunoId = null, DateOnly? inicio = null, DateOnly? fim = null)
        {
            await using var connection = await GetOpenConnectionAsync();

            var sb = new System.Text.StringBuilder();
            sb.Append($"SELECT * FROM {TableName} WHERE 1=1 ");

            if (alunoId.HasValue) sb.Append("AND aluno_id = @AlunoId ");
            if (inicio.HasValue) sb.Append("AND entrada >= @Inicio ");
            if (fim.HasValue) sb.Append("AND entrada <= @Fim ");

            await using var cmd = DbProvider.CreateCommand(sb.ToString(), connection);

            if (alunoId.HasValue)
                cmd.Parameters.Add(DbProvider.CreateParameter("@AlunoId", alunoId.Value, DbType.Int32, _databaseType));

            if (inicio.HasValue)
                cmd.Parameters.Add(DbProvider.CreateParameter("@Inicio", inicio.Value.ToDateTime(TimeOnly.MinValue), DbType.DateTime, _databaseType));

            if (fim.HasValue)
                cmd.Parameters.Add(DbProvider.CreateParameter("@Fim", fim.Value.ToDateTime(TimeOnly.MaxValue), DbType.DateTime, _databaseType));

            var list = new List<Acesso>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(await MapAsync(reader));

            return list;
        }

        public async Task<IEnumerable<Acesso>> GetAcessosPorColaboradorPeriodoAsync(int? colaboradorId = null, DateOnly? inicio = null, DateOnly? fim = null)
        {
            await using var connection = await GetOpenConnectionAsync();

            var sb = new System.Text.StringBuilder();
            sb.Append($"SELECT * FROM {TableName} WHERE 1=1 ");

            if (colaboradorId.HasValue) sb.Append("AND colaborador_id = @ColaboradorId ");
            if (inicio.HasValue) sb.Append("AND entrada >= @Inicio ");
            if (fim.HasValue) sb.Append("AND entrada <= @Fim ");

            await using var cmd = DbProvider.CreateCommand(sb.ToString(), connection);

            if (colaboradorId.HasValue)
                cmd.Parameters.Add(DbProvider.CreateParameter("@ColaboradorId", colaboradorId.Value, DbType.Int32, _databaseType));

            if (inicio.HasValue)
                cmd.Parameters.Add(DbProvider.CreateParameter("@Inicio", inicio.Value.ToDateTime(TimeOnly.MinValue), DbType.DateTime, _databaseType));

            if (fim.HasValue)
                cmd.Parameters.Add(DbProvider.CreateParameter("@Fim", fim.Value.ToDateTime(TimeOnly.MaxValue), DbType.DateTime, _databaseType));

            var list = new List<Acesso>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(await MapAsync(reader));

            return list;
        }

        public async Task<Dictionary<TimeOnly, int>> GetHorarioMaisProcuradoPorMesAsync(int mes)
        {
            await using var connection = await GetOpenConnectionAsync();

            string sql = _databaseType == DatabaseType.SqlServer
                ? $"SELECT DATEPART(HOUR, entrada) AS hora, COUNT(1) AS quantidade FROM {TableName} WHERE DATEPART(MONTH, entrada) = @Mes GROUP BY DATEPART(HOUR, entrada)"
                : $"SELECT HOUR(entrada) AS hora, COUNT(1) AS quantidade FROM {TableName} WHERE MONTH(entrada) = @Mes GROUP BY HOUR(entrada)";

            await using var cmd = DbProvider.CreateCommand(sql, connection);
            cmd.Parameters.Add(DbProvider.CreateParameter("@Mes", mes, DbType.Int32, _databaseType));

            var dict = new Dictionary<TimeOnly, int>();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int hora = Convert.ToInt32(reader["hora"]);
                int qt = Convert.ToInt32(reader["quantidade"]);
                dict[TimeOnly.FromTimeSpan(TimeSpan.FromHours(hora))] = qt;
            }

            return dict;
        }

        public async Task<Dictionary<int, TimeSpan>> GetPermanenciaMediaPorMesAsync(int mes)
        {
            await using var connection = await GetOpenConnectionAsync();

            string sql = _databaseType == DatabaseType.SqlServer
                ? $"SELECT DATEPART(MONTH, entrada) AS mes, AVG(CAST(DATEDIFF(MINUTE, entrada, saida) AS FLOAT)) AS mediaMinutos FROM {TableName} WHERE saida IS NOT NULL AND DATEPART(MONTH, entrada) = @Mes GROUP BY DATEPART(MONTH, entrada)"
                : $"SELECT MONTH(entrada) AS mes, AVG(TIMESTAMPDIFF(MINUTE, entrada, saida)) AS mediaMinutos FROM {TableName} WHERE saida IS NOT NULL AND MONTH(entrada) = @Mes GROUP BY MONTH(entrada)";

            await using var cmd = DbProvider.CreateCommand(sql, connection);
            cmd.Parameters.Add(DbProvider.CreateParameter("@Mes", mes, DbType.Int32, _databaseType));

            var dict = new Dictionary<int, TimeSpan>();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                double minutos = Convert.ToDouble(reader["mediaMinutos"]);
                dict[mes] = TimeSpan.FromMinutes(minutos);
            }

            return dict;
        }

        public async Task<IEnumerable<Aluno>> GetAlunosSemAcessoNosUltimosDiasAsync(int dias)
        {
            await using var connection = await GetOpenConnectionAsync();

            string sql = _databaseType == DatabaseType.SqlServer
                ? $"SELECT DISTINCT aluno_id FROM {TableName} WHERE entrada >= DATEADD(day, -@Dias, GETDATE())"
                : $"SELECT DISTINCT aluno_id FROM {TableName} WHERE entrada >= DATE_SUB(CURDATE(), INTERVAL @Dias DAY)";

            await using var cmd1 = DbProvider.CreateCommand(sql, connection);
            cmd1.Parameters.Add(DbProvider.CreateParameter("@Dias", dias, DbType.Int32, _databaseType));

            var acessos = new HashSet<int>();
            using (var rdr = await cmd1.ExecuteReaderAsync())
            {
                while (await rdr.ReadAsync())
                    acessos.Add(Convert.ToInt32(rdr["aluno_id"]));
            }

            string sqlAlunos = acessos.Count == 0
                ? "SELECT * FROM tb_aluno"
                : $"SELECT * FROM tb_aluno WHERE id_aluno NOT IN ({string.Join(",", acessos)})";

            await using var cmd2 = DbProvider.CreateCommand(sqlAlunos, connection);

            var list = new List<Aluno>();
            using var reader = await cmd2.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var aluno = CriarDummyAluno();
                typeof(Entity).GetProperty("Id")!.SetValue(aluno, Convert.ToInt32(reader["id_aluno"]));
                list.Add(aluno);
            }

            return list;
        }

        protected override async Task<Acesso> MapAsync(DbDataReader reader)
        {
            EPessoaTipo tipo = EPessoaTipo.Aluno;

            var acesso = CriarAcessoDummy(tipo, Convert.ToDateTime(reader["entrada"]));

            typeof(Entity).GetProperty("Id")!.SetValue(acesso, Convert.ToInt32(reader["id_acesso"]));

            acesso.AlunoId = reader["aluno_id"] is DBNull ? null : Convert.ToInt32(reader["aluno_id"]);
            acesso.ColaboradorId = reader["colaborador_id"] is DBNull ? null : Convert.ToInt32(reader["colaborador_id"]);
            acesso.Entrada = Convert.ToDateTime(reader["entrada"]);
            acesso.Saida = reader["saida"] is DBNull ? null : Convert.ToDateTime(reader["saida"]);

            return acesso;
        }

        public override Task<Acesso> Adicionar(Acesso entity)
        {
            throw new NotImplementedException();
        }

        public override Task<Acesso> Atualizar(Acesso entity)
        {
            throw new NotImplementedException();
        }
    }
}
