using mark.davison.common.persistence;

namespace mark.davison.edinburgh.api.persistence;

[ExcludeFromCodeCoverage]
public sealed class EdinburghDbContext : DbContextBase<EdinburghDbContext>
{
    public EdinburghDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);
    }

}
