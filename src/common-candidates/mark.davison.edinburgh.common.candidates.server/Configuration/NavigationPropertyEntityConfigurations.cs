using mark.davison.common.server.abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mark.davison.edinburgh.common.candidates.server.Configuration;

public static class NavigationPropertyEntityConfigurations
{
    public static void ConfigureEntity<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : BaseEntity
    {
        var properties = typeof(TEntity)
            .GetProperties()
            .Where(_ => _.Name != "Id" && _.Name.EndsWith("Id"));

        var entities = typeof(TEntity)
            .Assembly
            .DefinedTypes
            .Where(_ => _.BaseType == typeof(TEntity))
            .Select(_ => _.Name)
            .ToList();


        foreach (var property in properties)
        {
            var totalEntityMatch = property.Name.Substring(0, property.Name.Length - 2);
            var perfectMatch = entities.FirstOrDefault(_ => string.Equals(_, totalEntityMatch));
            if (perfectMatch == null)
            {
                perfectMatch = entities.FirstOrDefault(_ => totalEntityMatch.EndsWith(_));
            }

            if (perfectMatch != null)
            {
                builder
                    .HasOne($"{totalEntityMatch}")
                    .WithMany()
                    .HasForeignKey($"{totalEntityMatch}Id");
            }
        }

    }
}