namespace mark.davison.edinburgh.api.migrations.sqlite;

[ExcludeFromCodeCoverage]
public class ContextFactory : SqliteDbContextFactory<EdinburghDbContext>
{
    protected override EdinburghDbContext DbContextCreation(
            DbContextOptions<EdinburghDbContext> options
        ) => new EdinburghDbContext(options);
}
