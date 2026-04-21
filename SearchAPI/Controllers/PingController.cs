using Microsoft.AspNetCore.Mvc;

namespace SearchAPI.Controllers;

[ApiController]
[Route("ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    public string? Ping()
    {
        string? id = Environment.GetEnvironmentVariable("id");
        return $"Search API - ver 1.0 - instance {id}";
    }
    
}