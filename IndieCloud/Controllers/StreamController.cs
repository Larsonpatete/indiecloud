using IndieCloud.Models;
using Microsoft.AspNetCore.Mvc;

namespace IndieCloud.Controllers;

[ApiController]
[Route("[controller]")]
public class StreamController : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> UpdateStream([FromBody] Message message)
    {

    }
}
