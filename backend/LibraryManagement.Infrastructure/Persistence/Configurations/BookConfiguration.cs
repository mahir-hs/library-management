namespace LibraryManagement.Infrastructure.Persistence.Configurations;

using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("title");

        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(13)
            .HasColumnName("isbn");

        builder.Property(b => b.Description)
            .HasMaxLength(2000)
            .HasColumnName("description");

        builder.Property(b => b.Publisher)
            .HasMaxLength(255)
            .HasColumnName("publisher");

        builder.Property(b => b.PublishedYear)
            .HasColumnName("published_year");

        builder.Property(b => b.Language)
            .HasMaxLength(50)
            .HasColumnName("language");

        builder.Property(b => b.ImageUrl)
            .HasMaxLength(500)
            .HasColumnName("image_url");

        builder.Property(b => b.AuthorId)
            .IsRequired()
            .HasColumnName("author_id");

        builder.Property(b => b.CategoryId)
            .IsRequired()
            .HasColumnName("category_id");

        builder.Property(b => b.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(b => b.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(b => b.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(b => b.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(b => b.DeletedBy)
            .HasColumnName("deleted_by");

        // ============ UNIQUE CONSTRAINTS ============
        // ISBN must be globally unique
        builder.HasIndex(b => b.ISBN)
            .IsUnique()
            .HasDatabaseName("idx_books_isbn_unique");

        // ============ RELATIONSHIPS ============
        // Foreign keys are configured in Author and Category configurations (one-to-many)

        // One Book -> many BookCopies (physical copies)
        builder.HasMany(b => b.Copies)
            .WithOne(bc => bc.Book)
            .HasForeignKey(bc => bc.BookId)
            .OnDelete(DeleteBehavior.Cascade)  // Delete all copies when book is deleted
            .HasConstraintName("fk_book_copies_books");

        // One Book -> many Reservations (waitlist)
        builder.HasMany(b => b.Reservations)
            .WithOne(r => r.Book)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade)  // Delete reservations when book is deleted
            .HasConstraintName("fk_reservations_books");

        // ============ INDEXES FOR PERFORMANCE ============
        builder.HasIndex(b => b.Title)
            .HasDatabaseName("idx_books_title");

        builder.HasIndex(b => b.AuthorId)
            .HasDatabaseName("idx_books_author_id");

        builder.HasIndex(b => b.CategoryId)
            .HasDatabaseName("idx_books_category_id");
    }
}
