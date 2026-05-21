using System.Threading.Tasks;
using Xunit;
using MaphunziroBlackboard.Web.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace MaphunziroBlackboard.Tests;

public class BlobServiceTests
{
    [Fact]
    public async Task UploadNullFile_ReturnsNull()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>{{"BlobStorage:ConnectionString","UseDevelopmentStorage=true"}}).Build();
        var svc = new AzureBlobService(config);
        var result = await svc.UploadFileAsync(null, "assignments");
        Assert.Null(result);
    }
}
