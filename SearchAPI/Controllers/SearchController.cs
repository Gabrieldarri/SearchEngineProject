using Shared.Model;
using Microsoft.AspNetCore.Mvc;
using SearchAPI.Logic;
using System.IO;

namespace SearchAPI.Controllers;

[ApiController]
[Route("api")]
public class SearchController : ControllerBase
{
    // Use a composite database that queries both sqlite and postgres so
    // reception can split indexes between them. If Postgres is not available
    // fall back to a second sqlite database file so the API still works.
    private static IDatabase mDatabase;

    static SearchController()
    {
        IDatabase a = new DatabaseSqlite();
        IDatabase b = null;
        try
        {
            b = new DatabasePostgres();
        }
        catch
        {
            // Postgres not available — fall back to a second sqlite file
            var altPath = Shared.Paths.SQLITE_DATABASE + ".part2.db";
            b = new DatabaseSqlite(altPath);
        }

        mDatabase = new CompositeDatabase(a, b);
    }
    
    [HttpGet]
    [Route("search/{query}/{maxAmount}")]
    public SearchResult Search(string query, int maxAmount)
    {
        var logic = new SearchLogic(mDatabase);
        var result = logic.Search(query.Split(","), maxAmount);
        return result;
    }

    [HttpGet]
    [Route("ping")]
    public string? Ping()
    {
        return Environment.GetEnvironmentVariable("id");
    }
    
    [HttpGet]
    [Route("getfile")]
    public string GetFile([FromQuery] string path)
    {
        var uri = "file://" + path;
        var s = System.IO.File.ReadAllText(path);
        return s;
    }
    
    
}
