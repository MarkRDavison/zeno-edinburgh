namespace mark.davison.edinburgh.api.models.configuration.EntityConfiguration;

public sealed class SectionEntityConfiguration : EdinburghEntityConfiguration<Section>
{
    public override void ConfigureEntity(EntityTypeBuilder<Section> builder)
    {
        builder.Property(_ => _.Name);
    }
}
