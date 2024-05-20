namespace mark.davison.edinburgh.api.migrations.sqlite;

[ExcludeFromCodeCoverage]
//[DatabaseMigrationAssembly(DatabaseType.Sqlite)]
public class SqliteContextFactory : SqliteDbContextFactory<EdinburghDbContext>
{
    protected override EdinburghDbContext DbContextCreation(
            DbContextOptions<EdinburghDbContext> options
        ) => new EdinburghDbContext(options);
}
