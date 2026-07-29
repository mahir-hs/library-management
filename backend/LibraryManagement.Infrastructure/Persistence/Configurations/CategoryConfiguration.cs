namespace LibraryManagement.Infrastructure.Persistence.Configurations;

using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(c => c.Description)
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(c => c.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(c => c.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(c => c.DeletedBy)
            .HasColumnName("deleted_by");

        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("idx_categories_name_unique");

        builder.HasMany(c => c.Books)
            .WithOne(b => b.Category)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_books_categories");
    }
}