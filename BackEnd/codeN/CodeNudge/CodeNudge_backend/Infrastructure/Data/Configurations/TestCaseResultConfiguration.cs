using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Infrastructure.Data.Configurations;

public class TestCaseResultConfiguration : IEntityTypeConfiguration<TestCaseResult>
{
    public void Configure(EntityTypeBuilder<TestCaseResult> builder)
    {
        builder.ToTable("TestCaseResults");

        builder.HasKey(tcr => tcr.Id);

        // Properties
        builder.Property(tcr => tcr.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(tcr => tcr.ActualOutput)
            .HasColumnType("nvarchar(max)");

        builder.Property(tcr => tcr.ExecutionTime)
            .HasDefaultValue(0);

        builder.Property(tcr => tcr.MemoryUsed)
            .HasDefaultValue(0);

        builder.Property(tcr => tcr.ErrorMessage)
            .HasColumnType("nvarchar(max)");

        // Indexes
        builder.HasIndex(tcr => tcr.SubmissionId);
        builder.HasIndex(tcr => tcr.TestCaseId);

        // Relationships
        builder.HasOne(tcr => tcr.Submission)
            .WithMany(s => s.TestCaseResults)
            .HasForeignKey(tcr => tcr.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade); // Keep CASCADE for Submission

        builder.HasOne(tcr => tcr.TestCase)
            .WithMany()
            .HasForeignKey(tcr => tcr.TestCaseId)
            .OnDelete(DeleteBehavior.Restrict); // Changed from CASCADE to RESTRICT to avoid multiple cascade paths
    }
}
