using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MaphunziroBlackboard.Web.Services;

public class AzureBlobService : IBlobService
{
    private readonly BlobServiceClient _client;
    private readonly IConfiguration _configuration;

    public AzureBlobService(IConfiguration configuration)
    {
        _configuration = configuration;
        var conn = configuration["BlobStorage:ConnectionString"];
        if (string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException("BlobStorage:ConnectionString is not configured.");
        _client = new BlobServiceClient(conn);
    }

    public async Task<string> UploadFileAsync(IFormFile file, string containerName, string blobName = null)
    {
        if (file == null || file.Length == 0) return string.Empty;
        var container = _client.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None);
        blobName ??= Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var blob = container.GetBlobClient(blobName);

        using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, overwrite: true);
        return blob.Uri.ToString();
    }

    public async Task DeleteFileAsync(string containerName, string blobName)
    {
        var container = _client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        await blob.DeleteIfExistsAsync();
    }
}
