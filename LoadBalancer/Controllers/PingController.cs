using Microsoft.AspNetCore.Mvc;

namespace LoadBalancer.Controllers;

[ApiController]
[Route("ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    public string? Ping()
    {
        return Environment.GetEnvironmentVariable("id");
    }
    
}