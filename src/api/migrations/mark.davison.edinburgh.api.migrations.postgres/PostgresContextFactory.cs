namespace mark.davison.edinburgh.api.migrations.postgres;

[ExcludeFromCodeCoverage]
//[DatabaseMigrationAssembly(DatabaseType.Postgres)]
public class PostgresContextFactory : PostgresDbContextFactory<EdinburghDbContext>
{
    protected override string ConfigName => "DATABASE";

    protected override EdinburghDbContext DbContextCreation(
            DbContextOptions<EdinburghDbContext> options
        ) => new EdinburghDbContext(options);
}
