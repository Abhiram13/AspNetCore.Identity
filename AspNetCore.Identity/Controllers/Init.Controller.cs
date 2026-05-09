using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Identity.Controllers;

[ApiController]
[Route("")]
public class InitController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello. This is Identity Project");
    }
}