using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Reflection;
using Serilog;
using MediatR;
using FluentValidation;
using FluentValidation.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;

// Import application namespaces
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Infrastructure.Data;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Infrastructure.Data.Repositories;
using CodeNudge.Infrastructure.Services;
using CodeNudge.Infrastructure.External;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Presentation.Middleware;
using CodeNudge.Presentation.Hubs;
using AspNet.Security.OAuth.GitHub;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/codenudge-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

try
{
    Log.Information("Starting CodeNudge API application");

    // Add Entity Framework
    builder.Services.AddDbContext<CodeNudgeDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    // Add Identity (temporarily disabled for testing)
    // builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    //     .AddEntityFrameworkStores<CodeNudgeDbContext>()
    //     .AddDefaultTokenProviders();

    // JWT Configuration
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"];

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    })
    // Add Google OAuth
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["OAuth:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["OAuth:Google:ClientSecret"]!;
        options.CallbackPath = "/signin-google";
    })
    // Add GitHub OAuth
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["OAuth:GitHub:ClientId"]!;
        options.ClientSecret = builder.Configuration["OAuth:GitHub:ClientSecret"]!;
        options.CallbackPath = "/signin-github";
    });

    // Add CORS - More permissive for development
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                // Allow any origin in development
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
            else
            {
                // Restrict origins in production
                var corsSettings = builder.Configuration.GetSection("CorsSettings");
                var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200", "https://localhost:4200" };
                var allowCredentials = corsSettings.GetValue<bool>("AllowCredentials");

                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader();

                if (allowCredentials)
                {
                    policy.AllowCredentials();
                }
            }
        });
    });

    // Add MediatR (temporarily disabled for testing)
    // builder.Services.AddMediatR(cfg =>
    // {
    //     cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    // });

    // Add FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    // Add all repository services (temporarily disabled for testing)
    // builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    // builder.Services.AddScoped<IUserRepository, UserRepository>();
    // builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
    // builder.Services.AddScoped<IHRQuestionRepository, HRQuestionRepository>();
    // builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
    // builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
    // builder.Services.AddScoped<IInterviewExperienceRepository, InterviewExperienceRepository>();
    // builder.Services.AddScoped<IWeeklyChallengeRepository, WeeklyChallengeRepository>();
    // builder.Services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();

    // Add application services
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IOAuthService, OAuthService>();
    // builder.Services.AddScoped<ICodeExecutionService, Judge0Service>();
    // builder.Services.AddScoped<INotificationService, NotificationService>();
    // builder.Services.AddScoped<IFileUploadService, FileUploadService>();
    // builder.Services.AddScoped<IAIFeedbackService, AIFeedbackService>();
    // builder.Services.AddScoped<IAIEvaluationService, AIEvaluationService>();

    // Add Password Hasher
    builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

    // Add HTTP Client for Judge0
    builder.Services.AddHttpClient("Judge0Client", client =>
    {
        var judge0Settings = builder.Configuration.GetSection("Judge0Settings");
        client.BaseAddress = new Uri(judge0Settings["BaseUrl"]!);
        client.DefaultRequestHeaders.Add("X-RapidAPI-Key", judge0Settings["ApiKey"]);
        client.DefaultRequestHeaders.Add("X-RapidAPI-Host", judge0Settings["Host"]);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    // Add HTTP Client for OAuth
    builder.Services.AddHttpClient<IOAuthService, OAuthService>();

    // Add Memory Cache
    builder.Services.AddMemoryCache();

    // Add SignalR
    builder.Services.AddSignalR();

    // Add controllers
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "CodeNudge API",
            Version = "v1",
            Description = "A comprehensive interview preparation platform API"
        });

        // Add JWT Authentication to Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Configure file upload support
        options.OperationFilter<FileUploadOperationFilter>();

        // Configure custom schema IDs to avoid conflicts
        options.CustomSchemaIds(type =>
        {
            if (type.FullName != null)
            {
                // Use full namespace for schema ID to avoid conflicts
                return type.FullName.Replace("CodeNudge.Shared.Requests.", "")
                                   .Replace("CodeNudge.Shared.Responses.", "")
                                   .Replace(".", "");
            }
            return type.Name;
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeNudge API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseHttpsRedirection();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    // Add root endpoint for health check
    app.MapGet("/", () => new {
        message = "CodeNudge API is running",
        version = "1.0.0",
        timestamp = DateTime.UtcNow,
        status = "healthy"
    });

    app.MapControllers();

    // Map SignalR hubs
    app.MapHub<InterviewHub>("/hubs/interview");
    app.MapHub<NotificationHub>("/hubs/notification");

    // Initialize database (temporarily disabled for testing)
    // using (var scope = app.Services.CreateScope())
    // {
    //     try
    //     {
    //         var context = scope.ServiceProvider.GetRequiredService<CodeNudgeDbContext>();
    //         // Temporarily comment out database initialization to get server running
    //         // await DbInitializer.InitializeAsync(context);
    //         Log.Information("Database initialization skipped for testing");
    //     }
    //     catch (Exception ex)
    //     {
    //         Log.Warning(ex, "Database initialization failed, but continuing to start server");
    //     }
    // }

    Log.Information("CodeNudge API started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// File Upload Operation Filter for Swagger
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParameters = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(IFormFile[]))
            .ToList();

        if (fileParameters.Any())
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = fileParameters.ToDictionary(
                                p => p.Name!,
                                p => new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                })
                        }
                    }
                }
            };
        }
    }
}
