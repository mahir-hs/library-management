using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id");

        builder.Property(b => b.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(b => b.Code)
            .HasColumnName("code")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(b => b.Code)
            .IsUnique();

        builder.Property(b => b.Address)
            .HasColumnName("address")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(b => b.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(b => b.Email)
            .HasColumnName("email")
            .HasMaxLength(100);

        builder.Property(b => b.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(b => b.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
        builder.Property(b => b.UpdatedAt).HasColumnName("last_modified_at");
        builder.Property(b => b.UpdatedBy).HasColumnName("last_modified_by").HasMaxLength(100);
        builder.Property(b => b.DeletedAt).HasColumnName("deleted_at");
        builder.Property(b => b.DeletedBy).HasColumnName("deleted_by").HasMaxLength(100);

        builder.HasQueryFilter(b => b.DeletedAt == null);
    }
}
