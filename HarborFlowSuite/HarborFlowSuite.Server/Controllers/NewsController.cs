using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HarborFlowSuite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<NewsItemDto>>> GetNews()
        {
            var news = await _newsService.GetNewsAsync();
            return Ok(news);
        }
    }
}
