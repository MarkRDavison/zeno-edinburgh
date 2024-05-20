using mark.davison.common.persistence;

namespace mark.davison.edinburgh.api.migrations.postgres;

[ExcludeFromCodeCoverage]
[DatabaseMigrationAssembly(davison.common.persistence.Configuration.DatabaseType.Postgres)]
public class ContextFactory : PostgresDbContextFactory<EdinburghDbContext>
{
    protected override string ConfigName => "DATABASE";

    protected override EdinburghDbContext DbContextCreation(
            DbContextOptions<EdinburghDbContext> options
        ) => new EdinburghDbContext(options);
}
