namespace mark.davison.edinburgh.api.models.configuration.EntityConfiguration;

public sealed class BoardEntityConfiguration : EdinburghEntityConfiguration<Board>
{
    public override void ConfigureEntity(EntityTypeBuilder<Board> builder)
    {
        builder.Property(_ => _.Name);
    }
}
