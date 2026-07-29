namespace LibraryManagement.Infrastructure.Persistence.Configurations;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.ToTable("book_copies");

        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.BookId)
            .IsRequired()
            .HasColumnName("book_id");

        builder.Property(bc => bc.Barcode)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("barcode");

        builder.Property(bc => bc.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(BookCopyStatus.Available)
            .HasColumnName("status");

        builder.Property(bc => bc.ShelfLocation)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("shelf_location");

        builder.Property(bc => bc.AcquiredDate)
            .IsRequired()
            .HasColumnName("acquired_date");

        builder.Property(bc => bc.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(bc => bc.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(bc => bc.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(bc => bc.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(bc => bc.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(bc => bc.DeletedBy)
            .HasColumnName("deleted_by");

        builder.HasIndex(bc => bc.Barcode)
            .IsUnique()
            .HasDatabaseName("idx_book_copies_barcode_unique");

        builder.HasMany(bc => bc.BorrowRecords)
            .WithOne(br => br.BookCopy)
            .HasForeignKey(br => br.BookCopyId)
            .OnDelete(DeleteBehavior.Cascade)  
            .HasConstraintName("fk_borrow_records_book_copies");

        builder.HasIndex(bc => bc.BookId)
            .HasDatabaseName("idx_book_copies_book_id");

        builder.HasIndex(bc => bc.Status)
            .HasDatabaseName("idx_book_copies_status");

        builder.HasIndex(bc => new { bc.Status, bc.BookId })
            .HasDatabaseName("idx_book_copies_status_book_id");
    }
}