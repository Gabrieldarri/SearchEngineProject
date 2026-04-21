using Microsoft.AspNetCore.Mvc;
namespace LoadBalancer.Controllers;

// Note: This controller must be moved to its own API. Clear need for a y-split.
[ApiController]
[Route("api/file")]
public class FileController : ControllerBase
{
    // for testing only - should ask one of the instances...
    [HttpGet]
    [Route("get")]
    public string GetFile([FromQuery] string path)
    {
        var uri = "file://" + path;
        var s = System.IO.File.ReadAllText(path);
        return s;
    }
    
    
}
