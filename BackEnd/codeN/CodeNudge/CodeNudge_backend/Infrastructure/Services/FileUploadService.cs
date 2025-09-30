using CodeNudge.Core.Application.Interfaces;

namespace CodeNudge.Infrastructure.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private const long MaxImageSize = 5 * 1024 * 1024; // 5MB

    public FileUploadService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public async Task<string> UploadProfilePictureAsync(IFormFile file, Guid userId)
    {
        if (!IsValidImageAsync(file).Result)
        {
            throw new ArgumentException("Invalid image file");
        }

        var folder = "profile-pictures";
        var fileName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{GetFileExtension(file.FileName)}";
        
        return await UploadFileInternalAsync(file, folder, fileName);
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folder)
    {
        var fileName = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}{GetFileExtension(file.FileName)}";
        return await UploadFileInternalAsync(file, folder, fileName);
    }

    private async Task<string> UploadFileInternalAsync(IFormFile file, string folder, string fileName)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var filePath = Path.Combine(uploadsFolder, fileName);
        
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/uploads/{folder}/{fileName}";
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
                return Task.FromResult(false);

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<bool> IsValidImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        if (!IsValidFileSize(file, MaxImageSize))
            return false;

        if (!IsValidFileType(file, _allowedImageExtensions))
            return false;

        // Additional validation: check if it's actually an image
        try
        {
            using var stream = file.OpenReadStream();
            var buffer = new byte[8];
            await stream.ReadAsync(buffer, 0, 8);

            // Check for common image file signatures
            return IsValidImageSignature(buffer);
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidImageSignature(byte[] buffer)
    {
        // JPEG
        if (buffer.Length >= 2 && buffer[0] == 0xFF && buffer[1] == 0xD8)
            return true;

        // PNG
        if (buffer.Length >= 8 && buffer[0] == 0x89 && buffer[1] == 0x50 && 
            buffer[2] == 0x4E && buffer[3] == 0x47 && buffer[4] == 0x0D && 
            buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A)
            return true;

        // GIF
        if (buffer.Length >= 6 && buffer[0] == 0x47 && buffer[1] == 0x49 && 
            buffer[2] == 0x46 && buffer[3] == 0x38 && (buffer[4] == 0x37 || buffer[4] == 0x39) && 
            buffer[5] == 0x61)
            return true;

        return false;
    }

    public Task<string> GetFileUrlAsync(string filePath)
    {
        var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:7000";
        return Task.FromResult($"{baseUrl.TrimEnd('/')}{filePath}");
    }

    public string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant();
    }

    public bool IsValidFileSize(IFormFile file, long maxSizeInBytes)
    {
        return file?.Length <= maxSizeInBytes;
    }

    public bool IsValidFileType(IFormFile file, string[] allowedExtensions)
    {
        if (file == null) return false;
        
        var extension = GetFileExtension(file.FileName);
        return allowedExtensions.Contains(extension);
    }
}
