using URL_Shortener.Data;
using URL_Shortener.Entities;
using URL_Shortener.Models;
using URL_Shortener.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace URL_Shortener.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UrlShorteningController : Controller
    {
        private readonly UrlShorteningService _urlShorteningService;
        private readonly AuthDbContext _dbContext;

        public UrlShorteningController(UrlShorteningService urlShorteningService, AuthDbContext dbContext)
        {
            _urlShorteningService = urlShorteningService ?? throw new ArgumentNullException(nameof(urlShorteningService));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return BadRequest("The specified URL is invalid.");
            }

            var code = await _urlShorteningService.GenerateUniqueCode();

            if (await _dbContext.ShortUrls.AnyAsync(x => x.Code == code))
            {
                // Handle code conflict, e.g., generate a new code or return an error.
                return Conflict("Code conflict. Please try again.");
            }

            var shortenUrl = new ShortenedUrl
            {
                Id = Guid.NewGuid(),
                LongUrl = request.Url,
                Code = code,
                ShortUrl = $"{Request.Scheme}://{Request.Host}/api/{code}",
                CreatedOnUtc = DateTime.UtcNow
            };

            _dbContext.ShortUrls.Add(shortenUrl);
            await _dbContext.SaveChangesAsync();

            return Ok(shortenUrl.ShortUrl);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectShortenedUrl(string code)
        {
            var shortenedUrl = await _dbContext.ShortUrls.FirstOrDefaultAsync(x => x.Code == code);

            if (shortenedUrl == null)
            {
                return NotFound();
            }

            return Redirect(shortenedUrl.LongUrl);
        }
    }
}
