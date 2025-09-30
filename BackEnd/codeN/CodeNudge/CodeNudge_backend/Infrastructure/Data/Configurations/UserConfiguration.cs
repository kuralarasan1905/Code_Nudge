using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .HasConversion<int>()
            .HasDefaultValue(UserRole.Student);

        builder.Property(u => u.ProfilePicture)
            .HasMaxLength(500);

        builder.Property(u => u.College)
            .HasMaxLength(200);

        builder.Property(u => u.Branch)
            .HasMaxLength(100);

        // Role-specific identifiers
        builder.Property(u => u.RegisterId)
            .HasMaxLength(50);

        builder.Property(u => u.EmployeeId)
            .HasMaxLength(50);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(15);

        builder.Property(u => u.IsEmailVerified)
            .HasDefaultValue(false);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasMany(u => u.Submissions)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.InterviewSessions)
            .WithOne(i => i.HostUser)
            .HasForeignKey(i => i.HostUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.UserProgresses)
            .WithOne(up => up.User)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.InterviewExperiences)
            .WithOne(ie => ie.User)
            .HasForeignKey(ie => ie.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.LeaderboardEntries)
            .WithOne(le => le.User)
            .HasForeignKey(le => le.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
