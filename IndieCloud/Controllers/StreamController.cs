using IndieCloud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndieCloud.Controllers;

[ApiController]
[Route("[controller]")]
public class StreamController : ControllerBase
{
    private readonly AppDbContext _context;

    public StreamController(AppDbContext context)
    {
        _context = context;
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
        var messages = await _context.Messages
            .OrderByDescending(m => m.TimeStamp)
            .Take(limit)
            .ToListAsync();
        return Ok(messages);
    }
}
