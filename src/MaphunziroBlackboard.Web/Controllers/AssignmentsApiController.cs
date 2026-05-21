using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaphunziroBlackboard.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MaphunziroBlackboard.Web.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MaphunziroBlackboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace MaphunziroBlackboard.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignmentsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IBlobService _blobService;
    private readonly IConfiguration _configuration;

    public AssignmentsApiController(ApplicationDbContext context, IBlobService blobService, IConfiguration configuration)
    {
        _context = context;
        _blobService = blobService;
        _configuration = configuration;
    }

    [HttpPost("upload/{assignmentId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Upload(int assignmentId)
    {
        var file = Request.Form.Files.Count > 0 ? Request.Form.Files[0] : null;
        if (file == null) return BadRequest("No file provided");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
        if (student == null) return Forbid();

        var container = _configuration["BlobStorage:Container"] ?? "assignments";
        var upload = await _blobService.UploadFileAsync(file, container);
        if (upload == null) return StatusCode(500, "Upload failed");

        var submission = new AssignmentSubmission
        {
            AssignmentId = assignmentId,
            StudentId = student.Id,
            Title = "",
            FileName = file.FileName,
            FileSize = file.Length,
            FilePath = upload.Url,
            BlobName = upload.BlobName,
            SubmittedAt = DateTime.UtcNow,
            Status = SubmissionStatus.Submitted
        };

        _context.AssignmentSubmissions.Add(submission);
        await _context.SaveChangesAsync();
        return Ok(new { submission.Id, submission.FilePath });
    }
}
