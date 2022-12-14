using Microsoft.AspNetCore.Mvc;
using TinyUrl.Services;

namespace TinyUrl.Controllers;

[ApiController]
[Route("")]
public class UrlController : ControllerBase
{
    private readonly ILogger<UrlController> _logger;
    private readonly IShortenerService _shortenerService;

    public UrlController(ILogger<UrlController> logger, IShortenerService shortenerService)
    {
        _logger = logger;
        _shortenerService = shortenerService;
    }

    [HttpPut("{url}")]
    public async Task<ActionResult<string>> Shorten(string url)
    {
        if (url.Length > 2048)
        {
            return BadRequest("URL too long");
        }

        url = url.Replace("%2F", "/");

        var key = await _shortenerService.Shorten(url);
        var shortenedUrl = $"{Request.Scheme}://{Request.Host}/u/{key}";

        return Ok(shortenedUrl);
    }

    [HttpGet("u/{key}")]
    public async Task<ActionResult> RedirectFromKey(string key)
    {
        var url = Cache.Contains(key) 
            ? Cache.Get(key) 
            : await _shortenerService.Get(key);

        if (url == null)
        {
            return NotFound("Invalid URL");
        }

        Cache.Set(key, url);

        return new RedirectResult(url, true);
    }
}