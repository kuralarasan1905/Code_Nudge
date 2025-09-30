using Microsoft.EntityFrameworkCore;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;
using System.Text.Json;
using BCrypt.Net;

namespace CodeNudge.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(CodeNudgeDbContext context)
    {
        try
        {
            // Apply any pending migrations
            await context.Database.MigrateAsync();

            // Check if data already exists
            if (await context.Users.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Seed only basic admin user
            await SeedBasicUserAsync(context);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while initializing the database: {ex.Message}", ex);
        }
    }

    private static async Task SeedBasicUserAsync(CodeNudgeDbContext context)
    {
        var adminUser = new User
        {
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@codenudge.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            IsEmailVerified = true,
            IsActive = true,
            College = "CodeNudge University",
            Branch = "Computer Science",
            GraduationYear = 2024
        };

        await context.Users.AddAsync(adminUser);
    }

    private static async Task SeedUsersAsync(CodeNudgeDbContext context)
    {
        var users = new List<User>
        {
            new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@codenudge.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                IsEmailVerified = true,
                IsActive = true,
                College = "CodeNudge University",
                Branch = "Computer Science",
                GraduationYear = 2024
            },
            new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                Role = UserRole.Student,
                IsEmailVerified = true,
                IsActive = true,
                College = "Tech University",
                Branch = "Software Engineering",
                GraduationYear = 2025
            },
            new User
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                Role = UserRole.Student,
                IsEmailVerified = true,
                IsActive = true,
                College = "Engineering College",
                Branch = "Computer Science",
                GraduationYear = 2024
            }
        };

        await context.Users.AddRangeAsync(users);
    }

    private static async Task SeedHRQuestionsAsync(CodeNudgeDbContext context)
    {
        var hrQuestions = new List<HRQuestion>
        {
            new HRQuestion
            {
                Question = "Tell me about yourself.",
                Category = "Behavioral",
                Company = "General",
                ExpectedAnswer = "Provide a brief professional summary highlighting your background, skills, and career goals.",
                Tips = "Keep it concise, focus on professional aspects, and connect your experience to the role.",
                Tags = JsonSerializer.Serialize(new[] { "introduction", "behavioral", "common" }),
                IsActive = true
            },
            new HRQuestion
            {
                Question = "Why do you want to work for our company?",
                Category = "Company Culture",
                Company = "General",
                ExpectedAnswer = "Show that you've researched the company and explain how your values align with theirs.",
                Tips = "Research the company's mission, values, and recent achievements before answering.",
                Tags = JsonSerializer.Serialize(new[] { "company", "motivation", "research" }),
                IsActive = true
            },
            new HRQuestion
            {
                Question = "What are your strengths and weaknesses?",
                Category = "Strengths & Weaknesses",
                Company = "General",
                ExpectedAnswer = "Mention genuine strengths relevant to the role and a real weakness you're working to improve.",
                Tips = "For weaknesses, always mention what you're doing to improve them.",
                Tags = JsonSerializer.Serialize(new[] { "strengths", "weaknesses", "self-awareness" }),
                IsActive = true
            }
        };

        await context.HRQuestions.AddRangeAsync(hrQuestions);
    }

    private static async Task SeedQuestionsAsync(CodeNudgeDbContext context)
    {
        var questions = new List<Question>
        {
            new Question
            {
                Title = "Two Sum",
                Description = "Given an array of integers nums and an integer target, return indices of the two numbers such that they add up to target.",
                Type = QuestionType.Coding,
                Difficulty = DifficultyLevel.Easy,
                Category = "Array",
                Company = "Google",
                Points = 10,
                TimeLimit = 30,
                Tags = JsonSerializer.Serialize(new[] { "array", "hash-table", "easy" }),
                SampleInput = "[2,7,11,15]\n9",
                SampleOutput = "[0,1]",
                Explanation = "Because nums[0] + nums[1] == 9, we return [0, 1].",
                StarterCode = JsonSerializer.Serialize(new Dictionary<string, string>
                {
                    ["python"] = "def twoSum(nums, target):\n    # Your code here\n    pass",
                    ["javascript"] = "function twoSum(nums, target) {\n    // Your code here\n}",
                    ["java"] = "public int[] twoSum(int[] nums, int target) {\n    // Your code here\n    return new int[0];\n}"
                }),
                IsActive = true
            }
        };

        await context.Questions.AddRangeAsync(questions);
    }

    private static async Task SeedWeeklyChallengesAsync(CodeNudgeDbContext context)
    {
        var weeklyChallenge = new WeeklyChallenge
        {
            Title = "Array Mastery Challenge",
            Description = "Master array manipulation with these carefully selected problems.",
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow.AddDays(7),
            IsActive = true,
            MaxParticipants = 1000,
            PrizePool = 500
        };

        await context.WeeklyChallenges.AddAsync(weeklyChallenge);
    }
}
