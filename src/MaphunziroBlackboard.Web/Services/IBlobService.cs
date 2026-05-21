using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MaphunziroBlackboard.Web.Services;

public class BlobUploadResult
{
    public string Url { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;

    // Represents an empty upload result. Use this when an operation completed
    // but no blob was created (for example, when no file was provided).
    public static BlobUploadResult Empty { get; } = new BlobUploadResult();
}

public interface IBlobService
{
    Task<BlobUploadResult?> UploadFileAsync(IFormFile? file, string? containerName = null, string? blobName = null);
    Task DeleteFileAsync(string containerName, string blobName);
}
