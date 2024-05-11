﻿namespace mark.davison.edinburgh.api.models.configuration;

public abstract class EdinburghEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : EdinburghEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .ValueGeneratedNever();

        builder
            .Property(_ => _.Created);
        builder
            .Property(_ => _.LastModified);

        builder
            .HasOne(_ => _.User)
            .WithMany()
            .HasForeignKey(_ => _.UserId);

        if (!ConfigureNavigationManually)
        {
            NavigationPropertyEntityConfigurations.ConfigureEntity(builder);
        }
        ConfigureEntity(builder);
    }

    protected virtual bool ConfigureNavigationManually => false;

    public abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
}