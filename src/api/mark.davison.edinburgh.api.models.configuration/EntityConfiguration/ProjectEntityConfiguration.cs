namespace mark.davison.edinburgh.api.models.configuration.EntityConfiguration;

public sealed class ProjectEntityConfiguration : EdinburghEntityConfiguration<Project>
{
    public override void ConfigureEntity(EntityTypeBuilder<Project> builder)
    {
        builder.Property(_ => _.Name);
    }
}
