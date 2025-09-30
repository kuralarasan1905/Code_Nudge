using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Infrastructure.Data.Configurations;

public class ExperienceLikeConfiguration : IEntityTypeConfiguration<ExperienceLike>
{
    public void Configure(EntityTypeBuilder<ExperienceLike> builder)
    {
        builder.ToTable("ExperienceLikes");

        builder.HasKey(el => el.Id);

        // Indexes
        builder.HasIndex(el => el.UserId);
        builder.HasIndex(el => el.InterviewExperienceId);
        builder.HasIndex(el => new { el.UserId, el.InterviewExperienceId })
            .IsUnique(); // Ensure a user can only like an experience once

        // Relationships
        builder.HasOne(el => el.User)
            .WithMany()
            .HasForeignKey(el => el.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Changed from CASCADE to RESTRICT to avoid multiple cascade paths

        builder.HasOne(el => el.InterviewExperience)
            .WithMany(ie => ie.ExperienceLikes)
            .HasForeignKey(el => el.InterviewExperienceId)
            .OnDelete(DeleteBehavior.Cascade); // Keep CASCADE for InterviewExperience
    }
}
