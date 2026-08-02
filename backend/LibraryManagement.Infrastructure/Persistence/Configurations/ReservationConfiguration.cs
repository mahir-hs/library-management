using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.MemberId)
            .IsRequired()
            .HasColumnName("member_id");

        builder.Property(r => r.BookId)
            .IsRequired()
            .HasColumnName("book_id");

        builder.Property(r => r.PositionInQueue)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("position_in_queue");

        builder.Property(r => r.ReservedAt)
            .IsRequired()
            .HasColumnName("reserved_at");

        builder.Property(r => r.ExpiresAt)
            .HasColumnName("expires_at")
            .HasComment("Reservation expires 48 hours after being fulfilled (book held for member)");

        builder.Property(r => r.FulfilledAt)
            .HasColumnName("fulfilled_at");

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ReservationStatus.Pending)
            .HasColumnName("status");

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(r => r.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(r => r.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(r => r.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(r => r.DeletedBy)
            .HasColumnName("deleted_by");

        builder.HasIndex(r => r.MemberId)
            .HasDatabaseName("idx_reservations_member_id");

        builder.HasIndex(r => r.BookId)
            .HasDatabaseName("idx_reservations_book_id");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("idx_reservations_status");

        builder.HasIndex(r => new { r.BookId, r.Status, r.ReservedAt })
            .HasDatabaseName("idx_reservations_book_status_reserved");

        builder.HasIndex(r => new { r.MemberId, r.Status })
            .HasDatabaseName("idx_reservations_member_status");

        builder.HasIndex(r => r.ExpiresAt)
            .HasDatabaseName("idx_reservations_expires_at");
    }
}