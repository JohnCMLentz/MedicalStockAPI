using Microsoft.AspNetCore.Mvc;

namespace MedicalStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class SystemController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "Running",
            application = "MedicalStock API",
            timestamp = DateTime.UtcNow
        });
    }

}
