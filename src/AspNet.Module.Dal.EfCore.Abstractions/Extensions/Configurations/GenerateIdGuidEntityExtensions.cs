using AspNet.Module.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNet.Module.Dal.EfCore.Extensions.Configurations;

public static class GenerateIdGuidEntityExtensions
{
    public static EntityTypeBuilder<T> MapIdGuid<T>(this EntityTypeBuilder<T> builder)
        where T : class, IEntity<Guid>
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id").HasComment("ИД");
        return builder;
    }
}