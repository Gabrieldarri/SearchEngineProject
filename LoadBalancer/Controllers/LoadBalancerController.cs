using Microsoft.AspNetCore.Mvc;
using Shared.Model;

namespace LoadBalancer.Controllers;

[ApiController]
[Route("api")]
public class LoadBalancerController : ControllerBase
{
    private static int next = 0;
    private static string[] instances = Environment.GetEnvironmentVariable("instances").Split(';');

    
    [HttpGet]
    [Route("search/{query}/{maxAmount}")]
    public void Search(string query, int maxAmount)
    {
        string url = $"{instances[next]}/api/search/{query}/{maxAmount}";
        next = (next + 1) % instances.Length;
        Response.Redirect(url);
    }
    
    /*
    [HttpGet]
    [Route("search/{query}/{maxAmount}")]
    public async Task<SearchResult> Search(string query, int maxAmount)
    {
        HttpClient http = new HttpClient();
        string url = $"{instances[next]}/api/search/{query}/{maxAmount}";
        next = (next + 1) % instances.Length;
        var result = await http.GetFromJsonAsync<SearchResult>(url);
        return result;
    }
    */
}