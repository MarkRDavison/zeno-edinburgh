using mark.davison.common.persistence;

namespace mark.davison.edinburgh.api.migrations.sqlite;

[ExcludeFromCodeCoverage]
[DatabaseMigrationAssembly(davison.common.persistence.Configuration.DatabaseType.Sqlite)]
public class ContextFactory : SqliteDbContextFactory<EdinburghDbContext>
{
    protected override EdinburghDbContext DbContextCreation(
            DbContextOptions<EdinburghDbContext> options
        ) => new EdinburghDbContext(options);
}
