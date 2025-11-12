using BO.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using static BO.Dto.NewsArticleDtos;

namespace FUNewsManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagsController> _logger;

        public TagsController(
            ITagService tagService,
            ILogger<TagsController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        // ========================= PUBLIC ENDPOINTS =========================

  
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<List<TagResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTags()
        {
            try
            {
                var result = await _tagService.GetAllTagsAsync();
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tags");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
