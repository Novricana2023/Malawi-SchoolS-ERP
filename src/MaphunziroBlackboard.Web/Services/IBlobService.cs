using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MaphunziroBlackboard.Web.Services;

public interface IBlobService
{
    Task<string> UploadFileAsync(IFormFile file, string containerName, string blobName = null);
    Task DeleteFileAsync(string containerName, string blobName);
}
