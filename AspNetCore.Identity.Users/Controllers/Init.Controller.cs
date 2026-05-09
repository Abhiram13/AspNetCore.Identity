using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Identity.Controllers;

[ApiController]
[Route("")]
public class InitController : BaseApiController
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello. This is Identity Project");
    }
}