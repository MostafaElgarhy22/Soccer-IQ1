using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly HttpClient httpClient;

    public NewsController()
    {
        httpClient = new HttpClient();
    }

    [HttpGet]
    public async Task<IActionResult> GetLiveNews()
    {
        string apiKey = "7f6e437c2b964dfe99fab9bd1400d03e"; 
        string url = $"https://newsapi.org/v2/top-headlines?country=us&category=sports&apiKey={apiKey}";

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Failed to fetch news.");
        }

        var content = await response.Content.ReadAsStringAsync();
        return Content(content, "application/json");
    }
}
