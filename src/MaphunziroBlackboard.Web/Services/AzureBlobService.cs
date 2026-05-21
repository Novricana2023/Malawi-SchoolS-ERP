using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure.Storage.Sas;

namespace MaphunziroBlackboard.Web.Services;

public class AzureBlobService : IBlobService
{
    private readonly BlobServiceClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureBlobService> _logger;

    public AzureBlobService(IConfiguration configuration)
    {
        _configuration = configuration;
        var conn = configuration["BlobStorage:ConnectionString"];
        if (string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException("BlobStorage:ConnectionString is not configured.");
        _client = new BlobServiceClient(conn);
    }

    public async Task<BlobUploadResult?> UploadFileAsync(IFormFile file, string containerName, string? blobName = null)
    {
        if (file == null || file.Length == 0) return null;
        var container = _client.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
        blobName ??= Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var blob = container.GetBlobClient(blobName);

        using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, overwrite: true);
        _logger?.LogInformation("Uploaded blob {Blob} to container {Container}", blobName, containerName);
        return new BlobUploadResult { Url = blob.Uri.ToString(), BlobName = blobName };
    }

    public async Task DeleteFileAsync(string containerName, string blobName)
    {
        var container = _client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        await blob.DeleteIfExistsAsync();
        _logger?.LogInformation("Deleted blob {Blob} from container {Container}", blobName, containerName);
    }

    public string? GenerateBlobSasUrl(string containerName, string blobName, int validMinutes = 60)
    {
        try
        {
            var container = _client.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(blobName);
            if (!blob.Exists()) return null;

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(validMinutes),
                Resource = "b"
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            if (_client.CanGenerateAccountSasUri)
            {
                var sas = blob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(validMinutes));
                return sas.ToString();
            }
        }
        catch
        {
            // ignore
        }
        return null;
    }
}
