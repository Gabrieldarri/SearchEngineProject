using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.Model;

namespace Shared;

public class SearchLogicProxy
{
<<<<<<< HEAD
    private string serverEndPoint = "https://localhost:7137/api";
=======
    private string serverEndPoint = "http://localhost:5203/api";
>>>>>>> 952c038598b85b4b3737a8503e85da382baa041d

    private HttpClient mHttp;

    public SearchLogicProxy()
    {
        mHttp = new HttpClient();
    }

    public async Task<SearchResult> Search(string[] query, int maxAmount)
    {
        var completeUrl = $"{serverEndPoint}/search/{String.Join(",", query)}/{maxAmount}";
        return await mHttp.GetFromJsonAsync<SearchResult>(completeUrl);
    }

    public async Task<string> GetFileContent(string url)
    {
        var encodedurl = Uri.EscapeDataString(url);
        var completeUrl = $"{serverEndPoint}/file/get?path={encodedurl}";
        var fileContent = await mHttp.GetStringAsync(completeUrl);
        return fileContent;
    }
}