using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Infrastructure.External;

public class Judge0Service : ICodeExecutionService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<Judge0Service> _logger;
    private readonly string _baseUrl;
    private readonly string _apiKey;

    public Judge0Service(HttpClient httpClient, IConfiguration configuration, ILogger<Judge0Service> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _baseUrl = _configuration["Judge0Settings:BaseUrl"] ?? "https://judge0-ce.p.rapidapi.com";
        _apiKey = _configuration["Judge0Settings:ApiKey"] ?? "";

        // Configure HttpClient
        _httpClient.BaseAddress = new Uri(_baseUrl);
        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", "judge0-ce.p.rapidapi.com");
        }
    }

    public async Task<CodeExecutionResult> ExecuteCodeAsync(string code, ProgrammingLanguage language, string input, int timeLimit = 5, int memoryLimit = 128)
    {
        try
        {
            var languageId = GetLanguageId(language);
            
            var submissionRequest = new
            {
                source_code = Convert.ToBase64String(Encoding.UTF8.GetBytes(code)),
                language_id = languageId,
                stdin = Convert.ToBase64String(Encoding.UTF8.GetBytes(input)),
                cpu_time_limit = timeLimit,
                memory_limit = memoryLimit * 1024, // Convert MB to KB
                wall_time_limit = timeLimit + 2
            };

            var json = JsonSerializer.Serialize(submissionRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Submit code for execution
            var response = await _httpClient.PostAsync("/submissions?base64_encoded=true&wait=true", content);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Judge0 API returned error: {StatusCode}", response.StatusCode);
                return new CodeExecutionResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Code execution service unavailable",
                    Status = ExecutionStatus.InternalError
                };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Judge0Response>(responseContent);

            return MapToCodeExecutionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing code");
            return new CodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = "Internal error during code execution",
                Status = ExecutionStatus.InternalError
            };
        }
    }

    public async Task<List<TestCaseExecutionResult>> ExecuteTestCasesAsync(string code, ProgrammingLanguage language, List<TestCaseInput> testCases)
    {
        var results = new List<TestCaseExecutionResult>();

        foreach (var testCase in testCases)
        {
            var executionResult = await ExecuteCodeAsync(code, language, testCase.Input, testCase.TimeLimit, testCase.MemoryLimit);
            
            var testCaseResult = new TestCaseExecutionResult
            {
                Input = testCase.Input,
                ExpectedOutput = testCase.ExpectedOutput.Trim(),
                ActualOutput = executionResult.Output.Trim(),
                ExecutionTime = executionResult.ExecutionTime,
                MemoryUsed = executionResult.MemoryUsed,
                ErrorMessage = executionResult.ErrorMessage,
                Status = executionResult.Status,
                IsPassed = executionResult.IsSuccess && 
                          string.Equals(executionResult.Output.Trim(), testCase.ExpectedOutput.Trim(), StringComparison.OrdinalIgnoreCase)
            };

            results.Add(testCaseResult);

            // If there's a compilation error or runtime error, no need to continue with other test cases
            if (executionResult.Status == ExecutionStatus.CompilationError || 
                executionResult.Status == ExecutionStatus.RuntimeError)
            {
                break;
            }
        }

        return results;
    }

    public async Task<bool> IsServiceAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/languages");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public int GetLanguageId(ProgrammingLanguage language)
    {
        return language switch
        {
            ProgrammingLanguage.C => 50,           // C (GCC 9.2.0)
            ProgrammingLanguage.CPlusPlus => 54,   // C++ (GCC 9.2.0)
            ProgrammingLanguage.CSharp => 51,      // C# (Mono 6.6.0.161)
            ProgrammingLanguage.Java => 62,        // Java (OpenJDK 13.0.1)
            ProgrammingLanguage.Python => 71,      // Python (3.8.1)
            ProgrammingLanguage.JavaScript => 63,  // JavaScript (Node.js 12.14.0)
            ProgrammingLanguage.TypeScript => 74,  // TypeScript (3.7.4)
            ProgrammingLanguage.Go => 60,          // Go (1.13.5)
            ProgrammingLanguage.Rust => 73,        // Rust (1.40.0)
            ProgrammingLanguage.PHP => 68,         // PHP (7.4.1)
            ProgrammingLanguage.Ruby => 72,        // Ruby (2.7.0)
            ProgrammingLanguage.Swift => 83,       // Swift (5.2.3)
            ProgrammingLanguage.Kotlin => 78,      // Kotlin (1.3.70)
            _ => 71 // Default to Python
        };
    }

    private static CodeExecutionResult MapToCodeExecutionResult(Judge0Response? response)
    {
        if (response == null)
        {
            return new CodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = "No response from execution service",
                Status = ExecutionStatus.InternalError
            };
        }

        var output = string.Empty;
        var errorMessage = string.Empty;

        // Decode base64 outputs
        if (!string.IsNullOrEmpty(response.Stdout))
        {
            try
            {
                output = Encoding.UTF8.GetString(Convert.FromBase64String(response.Stdout));
            }
            catch
            {
                output = response.Stdout; // Fallback if not base64
            }
        }

        if (!string.IsNullOrEmpty(response.Stderr))
        {
            try
            {
                errorMessage = Encoding.UTF8.GetString(Convert.FromBase64String(response.Stderr));
            }
            catch
            {
                errorMessage = response.Stderr; // Fallback if not base64
            }
        }

        if (!string.IsNullOrEmpty(response.CompileOutput))
        {
            try
            {
                var compileError = Encoding.UTF8.GetString(Convert.FromBase64String(response.CompileOutput));
                errorMessage = string.IsNullOrEmpty(errorMessage) ? compileError : $"{errorMessage}\n{compileError}";
            }
            catch
            {
                errorMessage = string.IsNullOrEmpty(errorMessage) ? response.CompileOutput : $"{errorMessage}\n{response.CompileOutput}";
            }
        }

        var status = (ExecutionStatus)(response.Status?.Id ?? 13);
        var isSuccess = status == ExecutionStatus.Accepted;

        return new CodeExecutionResult
        {
            Output = output,
            ErrorMessage = string.IsNullOrEmpty(errorMessage) ? null : errorMessage,
            ExecutionTime = (int)((response.Time ?? 0) * 1000), // Convert seconds to milliseconds
            MemoryUsed = response.Memory ?? 0,
            IsSuccess = isSuccess,
            Status = status,
            CompileOutput = response.CompileOutput
        };
    }
}

// Judge0 API Response Models
public class Judge0Response
{
    public string? Stdout { get; set; }
    public string? Stderr { get; set; }
    public string? CompileOutput { get; set; }
    public double? Time { get; set; }
    public int? Memory { get; set; }
    public Judge0Status? Status { get; set; }
}

public class Judge0Status
{
    public int Id { get; set; }
    public string? Description { get; set; }
}
