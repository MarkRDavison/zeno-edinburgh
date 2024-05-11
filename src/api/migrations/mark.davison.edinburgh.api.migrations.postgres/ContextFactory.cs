namespace mark.davison.edinburgh.api.migrations.postgres;

[ExcludeFromCodeCoverage]
public class ContextFactory : PostgresDbContextFactory<EdinburghDbContext>
{
    protected override string ConfigName => "DATABASE";

    protected override EdinburghDbContext DbContextCreation(
            DbContextOptions<EdinburghDbContext> options
        ) => new EdinburghDbContext(options);
}
