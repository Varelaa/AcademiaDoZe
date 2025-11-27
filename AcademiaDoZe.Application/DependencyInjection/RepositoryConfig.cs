//Gabriel Souza Varela

using AcademiaDoZe.Application.Enums;

public class RepositoryConfig
{
    public required string ConnectionString { get; set; }
    public required EAppDatabaseType DatabaseType { get; set; }
}
