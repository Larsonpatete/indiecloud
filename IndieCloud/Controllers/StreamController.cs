using IndieCloud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IndieCloud.AllowedFiles;

namespace IndieCloud.Controllers;

[ApiController]
[Route("[controller]")]
public class StreamController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public StreamController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStream([FromBody] MessageDTO chat)
    {
        var message = new StreamObject
        {
            Type = StreamDataType.Text,
            TextContent = chat.Chat,
            TimeStamp = DateTime.UtcNow,
            Device = Request.Headers.UserAgent.ToString() ?? "Unkown"
        };

        _context.StreamObjects.Add(message);
        await _context.SaveChangesAsync();

        return Ok(message);
    }

    [HttpGet]
    public async Task<IActionResult> GetStream([FromQuery] int limit = 10)
    {
        var messages = await _context.StreamObjects
            .OrderByDescending(s => s.TimeStamp)
            .Take(limit)
            .ToListAsync();
        return Ok(messages);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> PostFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (file.Length > 15 * 1024 * 1024) // Limit to 10MB
            return BadRequest("File size exceeds the 10MB limit.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        

        if (!AllowedFiles.AllowedFiles.MimeMap.ContainsKey(ext))
            return BadRequest("File type not allowed.");

        var uploadPath = _config["Storage:UploadPath"];        
        Directory.CreateDirectory(uploadPath);

        var storedName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadPath, storedName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var streamObject = new StreamObject
        {
            Type = GetDataType(ext),
            FileName = storedName,
            TimeStamp = DateTime.UtcNow,
            Device = Request.Headers.UserAgent.ToString() ?? "Unkown"
        };

        _context.StreamObjects.Add(streamObject);
        await _context.SaveChangesAsync();

        return Ok(new {FileName = storedName, file.ContentType, file.Length});
    }

    [HttpGet("uploads/{fileName}")]
    public async Task<IActionResult> GetFile(string fileName) {
      var uploadPath = _config["Storage:UploadPath"];
      var filePath = Path.Combine(uploadPath, fileName);

      if (!System.IO.File.Exists(filePath))
        return NotFound();

      var ext = Path.GetExtension(fileName).ToLowerInvariant();
      var mimeType = AllowedFiles.AllowedFiles.MimeMap.GetValueOrDefault(ext, "application/octet-stream");

      return PhysicalFile(filePath, mimeType);
    }

    private static StreamDataType GetDataType(string? extension) => extension switch
    {
        ".wav" or ".mp3" => StreamDataType.Audio,
        _ => StreamDataType.Image
    };
}
