using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class BorrowRecordConfiguration : IEntityTypeConfiguration<BorrowRecord>
{
    public void Configure(EntityTypeBuilder<BorrowRecord> builder)
    {
        builder.ToTable("borrow_records");

        builder.HasKey(br => br.Id);

        builder.Property(br => br.MemberId)
            .IsRequired()
            .HasColumnName("member_id");

        builder.Property(br => br.BookCopyId)
            .IsRequired()
            .HasColumnName("book_copy_id");

        builder.Property(br => br.BorrowedAt)
            .IsRequired()
            .HasColumnName("borrowed_at");

        builder.Property(br => br.DueDate)
            .IsRequired()
            .HasColumnName("due_date");

        builder.Property(br => br.ReturnedAt)
            .HasColumnName("returned_at");

        builder.Property(br => br.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(BorrowStatus.Borrowed)
            .HasColumnName("status");

        builder.Property(br => br.FineAmount)
            .HasPrecision(10, 2)
            .HasDefaultValue(0m)
            .HasColumnName("fine_amount");

        builder.Property(br => br.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(br => br.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(br => br.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(br => br.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(br => br.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(br => br.DeletedBy)
            .HasColumnName("deleted_by");

        builder.HasIndex(br => br.MemberId)
            .HasDatabaseName("idx_borrow_records_member_id");

        builder.HasIndex(br => br.BookCopyId)
            .HasDatabaseName("idx_borrow_records_book_copy_id");

        builder.HasIndex(br => br.Status)
            .HasDatabaseName("idx_borrow_records_status");

        builder.HasIndex(br => new { br.MemberId, br.Status })
            .HasDatabaseName("idx_borrow_records_member_status");

        builder.HasIndex(br => br.DueDate)
            .HasDatabaseName("idx_borrow_records_due_date");

        builder.HasIndex(br => new { br.Status, br.DueDate })
            .HasDatabaseName("idx_borrow_records_status_due_date");
    }
}