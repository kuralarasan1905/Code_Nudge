using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Infrastructure.Data.Configurations;

public class InterviewSessionConfiguration : IEntityTypeConfiguration<InterviewSession>
{
    public void Configure(EntityTypeBuilder<InterviewSession> builder)
    {
        builder.ToTable("InterviewSessions");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(i => i.Status)
            .HasConversion<int>()
            .HasDefaultValue(InterviewStatus.Scheduled);

        builder.Property(i => i.RoomCode)
            .HasMaxLength(20);

        builder.Property(i => i.Duration)
            .HasDefaultValue(60);

        builder.Property(i => i.Notes)
            .HasColumnType("nvarchar(max)");

        builder.Property(i => i.Feedback)
            .HasColumnType("nvarchar(max)");

        builder.Property(i => i.IsPublic)
            .HasDefaultValue(false);

        builder.Property(i => i.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(i => i.HostUserId);
        builder.HasIndex(i => i.ParticipantUserId);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.ScheduledAt);
        builder.HasIndex(i => i.RoomCode);

        // Relationships
        builder.HasOne(i => i.HostUser)
            .WithMany(u => u.InterviewSessions)
            .HasForeignKey(i => i.HostUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.ParticipantUser)
            .WithMany()
            .HasForeignKey(i => i.ParticipantUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(i => i.InterviewQuestions)
            .WithOne(iq => iq.InterviewSession)
            .HasForeignKey(iq => iq.InterviewSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
