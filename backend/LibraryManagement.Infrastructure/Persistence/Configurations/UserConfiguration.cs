namespace LibraryManagement.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryManagement.Domain.Entities;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .HasComment("Unique identifier - UUID v7");

        builder.HasQueryFilter(u => u.DeletedAt == null);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("username");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("email")
            .HasComment("User's email - must be unique and valid format");

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasColumnName("password_hash")
            .HasComment("BCrypt hashed password - never store plaintext");

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("full_name");

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20)
            .HasColumnName("phone_number");

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("role")
            .HasComment("User role: Admin (1), Librarian (2), Member (3)");

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active")
            .HasComment("Whether user account is active");

        builder.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at");

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(u => u.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(u => u.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(u => u.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(u => u.DeletedBy)
            .HasColumnName("deleted_by");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("idx_users_email_unique");

        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("idx_users_username_unique");

        builder.HasOne(u => u.Member)
            .WithOne(m => m.User)
            .HasForeignKey<Member>(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_members_users");

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_refresh_tokens_users");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("idx_users_is_active");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("idx_users_created_at");
    }
}