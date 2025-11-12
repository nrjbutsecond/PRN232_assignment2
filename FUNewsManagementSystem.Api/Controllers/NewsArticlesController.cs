using BO.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Security.Claims;
using static BO.Dto.NewsArticleDtos;

namespace FUNewsManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticlesController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        private int? GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (int.TryParse(roleClaim, out int role))
            {
                return role;
            }
            return null;
        }

        /// <summary>
        /// Get all active news articles (Public)
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveNews()
        {
            try
            {
                var result = await _newsArticleService.GetActiveNewsArticlesAsync();
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get news article by ID (Public for active, Staff for their own inactive)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsById(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _newsArticleService.GetNewsArticleByIdAsync(id, currentUserId);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // ========================= STAFF ENDPOINTS =========================

        /// <summary>
        /// Get all news articles (Staff/Admin)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetAllNews()
        {
            try
            {
                var result = await _newsArticleService.GetAllNewsArticlesAsync();
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get my news articles (Staff - view history)
        /// </summary>
        [HttpGet("my-articles")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetMyNews()
        {
            try
            {
                var staffId = GetCurrentUserId();
                if (!staffId.HasValue)
                {
                    return Unauthorized("User not authenticated");
                }

                var result = await _newsArticleService.GetMyNewsArticlesAsync(staffId.Value);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Search news articles (Staff/Admin)
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> SearchNews([FromQuery] string? keyword)
        {
            try
            {
                var result = await _newsArticleService.SearchNewsArticlesAsync(keyword ?? string.Empty);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create news article (Staff only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CreateNews([FromBody] CreateNewsArticleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { errors });
                }

                var staffId = GetCurrentUserId();
                if (!staffId.HasValue)
                {
                    return Unauthorized("User not authenticated");
                }

                var result = await _newsArticleService.CreateNewsArticleAsync(request, staffId.Value);

                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetNewsById), new { id = result.Data?.NewsArticleId }, result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update news article (Staff - own articles only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] UpdateNewsArticleRequest request)
        {
            try
            {
                if (id != request.NewsArticleId)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { errors });
                }

                var staffId = GetCurrentUserId();
                if (!staffId.HasValue)
                {
                    return Unauthorized("User not authenticated");
                }

                var result = await _newsArticleService.UpdateNewsArticleAsync(request, staffId.Value);

                if (result.Success)
                {
                    return Ok(result);
                }

                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete news article (Staff - own articles only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            try
            {
                var staffId = GetCurrentUserId();
                if (!staffId.HasValue)
                {
                    return Unauthorized("User not authenticated");
                }

                var result = await _newsArticleService.DeleteNewsArticleAsync(id, staffId.Value);

                if (result.Success)
                {
                    return Ok(result);
                }

                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}