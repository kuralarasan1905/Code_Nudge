using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Infrastructure.Data.Configurations;

public class HRQuestionConfiguration : IEntityTypeConfiguration<HRQuestion>
{
    public void Configure(EntityTypeBuilder<HRQuestion> builder)
    {
        builder.ToTable("HRQuestions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Question)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Company)
            .HasMaxLength(100);

        builder.Property(x => x.ExpectedAnswer)
            .HasMaxLength(2000);

        builder.Property(x => x.Tips)
            .HasMaxLength(1000);

        builder.Property(x => x.Tags)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.ViewCount)
            .HasDefaultValue(0);

        builder.Property(x => x.LikeCount)
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(450);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(450);

        // Indexes
        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.Company);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.CreatedAt);
    }
}
