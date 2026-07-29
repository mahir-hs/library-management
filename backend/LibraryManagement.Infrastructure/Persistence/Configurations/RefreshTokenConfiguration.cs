using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("token");

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired()
            .HasColumnName("expires_at");

        builder.Property(rt => rt.RevokedAt)
            .HasColumnName("revoked_at")
            .HasComment("If set, token has been revoked and cannot be used");

        builder.HasIndex(rt => rt.Token)
            .IsUnique()
            .HasDatabaseName("idx_refresh_tokens_token_unique");

        builder.HasIndex(rt => rt.UserId)
            .HasDatabaseName("idx_refresh_tokens_user_id");

        builder.HasIndex(rt => rt.ExpiresAt)
            .HasDatabaseName("idx_refresh_tokens_expires_at");

        builder.HasIndex(rt => new { rt.UserId, rt.ExpiresAt, rt.RevokedAt })
            .HasDatabaseName("idx_refresh_tokens_user_expiry_revoked");
    }
}