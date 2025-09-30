using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Infrastructure.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(q => q.Description)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(q => q.Difficulty)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(q => q.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(q => q.Company)
            .HasMaxLength(100);

        builder.Property(q => q.Points)
            .HasDefaultValue(10);

        builder.Property(q => q.TimeLimit)
            .HasDefaultValue(60);

        builder.Property(q => q.IsActive)
            .HasDefaultValue(true);

        builder.Property(q => q.Tags)
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.Hints)
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.SampleInput)
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.SampleOutput)
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.Explanation)
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.StarterCode)
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.Solution)
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.Options)
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.CorrectAnswer)
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(q => q.Type);
        builder.HasIndex(q => q.Difficulty);
        builder.HasIndex(q => q.Category);
        builder.HasIndex(q => q.Company);
        builder.HasIndex(q => q.IsActive);

        // Relationships
        builder.HasMany(q => q.TestCases)
            .WithOne(tc => tc.Question)
            .HasForeignKey(tc => tc.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.Submissions)
            .WithOne(s => s.Question)
            .HasForeignKey(s => s.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.UserProgresses)
            .WithOne(up => up.Question)
            .HasForeignKey(up => up.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
