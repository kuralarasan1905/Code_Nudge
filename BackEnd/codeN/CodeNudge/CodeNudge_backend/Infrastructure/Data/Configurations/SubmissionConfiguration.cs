using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Infrastructure.Data.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.Language)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<int>()
            .HasDefaultValue(SubmissionStatus.Pending);

        builder.Property(s => s.TestCasesPassed)
            .HasDefaultValue(0);

        builder.Property(s => s.TotalTestCases)
            .HasDefaultValue(0);

        builder.Property(s => s.ExecutionTime)
            .HasDefaultValue(0);

        builder.Property(s => s.MemoryUsed)
            .HasDefaultValue(0);

        builder.Property(s => s.ErrorMessage)
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.Output)
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.SubmittedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.QuestionId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.SubmittedAt);

        // Relationships
        builder.HasOne(s => s.User)
            .WithMany(u => u.Submissions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Question)
            .WithMany(q => q.Submissions)
            .HasForeignKey(s => s.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.TestCaseResults)
            .WithOne(tcr => tcr.Submission)
            .HasForeignKey(tcr => tcr.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
