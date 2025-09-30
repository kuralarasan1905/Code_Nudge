namespace CodeNudge.Core.Application.Interfaces;

public interface IFileUploadService
{
    Task<string> UploadProfilePictureAsync(IFormFile file, Guid userId);
    Task<string> UploadFileAsync(IFormFile file, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> IsValidImageAsync(IFormFile file);
    Task<string> GetFileUrlAsync(string filePath);
    string GetFileExtension(string fileName);
    bool IsValidFileSize(IFormFile file, long maxSizeInBytes);
    bool IsValidFileType(IFormFile file, string[] allowedExtensions);
}

public class FileUploadResult
{
    public bool Success { get; set; }
    public string? FilePath { get; set; }
    public string? FileUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public long FileSize { get; set; }
    public string? FileName { get; set; }
}
