using IndieCloud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        var message = new Message
        {
            Chat = chat.Chat,
            TimeStamp = DateTime.UtcNow,
            Device = Request.Headers.UserAgent.ToString() ?? "Unkown"
        };

        _context.Messages.Add(message);
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

        if (file.Length > 10 * 1024 * 1024) // Limit to 10MB
            return BadRequest("File size exceeds the 10MB limit.");

        var allowed = new [] { ".jpg", ".jpeg", ".png", ".gif"};
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowed.Contains(ext))
            return BadRequest("File type not allowed.");

        var uploadPath = _config["Storage:UploadPath"];        
        Directory.CreateDirectory(uploadPath);

        var storedName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadPath, storedName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(new {FileName = storedName, file.ContentType, file.Length});
    }

    [HttpGet("uploads/{fileName}")]
    public IActionREsult<Task> GetFile(string fileName) {
      var uploadPath = _config["Storage:UploadPath"];
      var filePath = Path.Combine(uploadPath, fileName);

      if (!System.IO.File.Exists(filePath))
        return NotFound();

      var ext = Path.GetExtension(fileName).ToLowerInvariant();
      var mimeType = ext switch {
        ".jpg" or "jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        - => "application/octet-stream"
      };

      return PhysicalFile(filePath, mimeType);
    }
}
