namespace mark.davison.edinburgh.api.models.configuration.EntityConfiguration;

public sealed class CardEntityConfiguration : EdinburghEntityConfiguration<Card>
{
    public override void ConfigureEntity(EntityTypeBuilder<Card> builder)
    {
        builder.Property(_ => _.Title);
    }
}
