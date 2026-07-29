namespace LibraryManagement.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryManagement.Domain.Entities;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");

        builder.HasQueryFilter(m => m.DeletedAt == null);

        builder.Property(m => m.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(m => m.MembershipNumber)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("membership_number");

        builder.Property(m => m.Address)
            .HasMaxLength(500)
            .HasColumnName("address");

        builder.Property(m => m.JoinedDate)
            .IsRequired()
            .HasColumnName("member_join_date");

        builder.Property(m => m.MaxAllowedBorrows)
            .HasDefaultValue(5)
            .HasColumnName("max_allowed_borrows");

        // Audit Columns
        builder.Property(m => m.CreatedAt).IsRequired().HasColumnName("created_at");
        builder.Property(m => m.CreatedBy).HasColumnName("created_by");
        builder.Property(m => m.UpdatedAt).HasColumnName("updated_at");
        builder.Property(m => m.UpdatedBy).HasColumnName("updated_by");
        builder.Property(m => m.DeletedAt).HasColumnName("deleted_at");
        builder.Property(m => m.DeletedBy).HasColumnName("deleted_by");

        // Indexes & Constraints
        builder.HasIndex(m => m.MembershipNumber)
            .IsUnique()
            .HasDatabaseName("idx_members_membership_number_unique");

        builder.HasIndex(m => m.UserId)
            .IsUnique()
            .HasDatabaseName("idx_members_user_id_unique");
    }
}