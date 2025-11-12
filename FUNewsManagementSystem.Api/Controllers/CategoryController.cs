using BO.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace FUNewsManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // ========================= PUBLIC ENDPOINTS =========================
        [HttpGet("active")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<List<CategorySimpleResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveCategories()
        {
            try
            {
                var result = await _categoryService.GetActiveCategoriesAsync();

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<CategorySimpleResponse>>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpGet("tree")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryTreeResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategoryTree()
        {
            try
            {
                var result = await _categoryService.GetCategoryTreeAsync();

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<CategoryTreeResponse>>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }

        // ========================= STAFF + ADMIN ENDPOINTS =========================

        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryListResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var result = await _categoryService.GetAllCategoriesAsync();

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<CategoryListResponse>>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCategoryById(short id)
        {
            try
            {
                var result = await _categoryService.GetCategoryByIdAsync(id);

                if (result.Success)
                {
                    return Ok(result);
                }

                return NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CategoryDetailResponse>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpGet("search")]
        [Authorize(Roles = "Staff,Admin")]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryListResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SearchCategories([FromQuery] string keyword)
        {
            try
            {
                var result = await _categoryService.SearchCategoriesAsync(keyword ?? string.Empty);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<CategoryListResponse>>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpPost("search/paged")]
        [Authorize(Roles = "Staff,Admin")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CategoryListResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedCategories([FromBody] CategorySearchRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<PagedResult<CategoryListResponse>>.ErrorResponse(
                        "Validation failed",
                        errors
                    ));
                }

                var result = await _categoryService.GetPagedCategoriesAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PagedResult<CategoryListResponse>>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }


        [HttpGet("{parentId}/subcategories")]
        [Authorize(Roles = "Staff,Admin")]
        [ProducesResponseType(typeof(ApiResponse<List<CategorySimpleResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubCategories(short parentId)
        {
            try
            {
                var result = await _categoryService.GetSubCategoriesAsync(parentId);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<CategorySimpleResponse>>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }

        // ========================= STAFF ONLY ENDPOINTS (CRUD) =========================

        [HttpPost]
        [Authorize(Roles = "Staff")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDetailResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<CategoryDetailResponse>.ErrorResponse(
                        "Validation failed",
                        errors
                    ));
                }

                var result = await _categoryService.CreateCategoryAsync(request);

                if (result.Success)
                {
                    return CreatedAtAction(
                        nameof(GetCategoryById),
                        new { id = result.Data?.CategoryId },
                        result
                    );
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CategoryDetailResponse>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCategory(short id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                if (id != request.CategoryId)
                {
                    return BadRequest(ApiResponse<CategoryDetailResponse>.ErrorResponse(
                        "Category ID mismatch",
                        new List<string> { "URL ID does not match request body ID" }
                    ));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<CategoryDetailResponse>.ErrorResponse(
                        "Validation failed",
                        errors
                    ));
                }

                var result = await _categoryService.UpdateCategoryAsync(request);

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
                return StatusCode(500, ApiResponse<CategoryDetailResponse>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }

 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteCategory(short id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);

                if (result.Success)
                {
                    return Ok(result);
                }

                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }

                // Category has dependencies (news articles or sub-categories)
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }


        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "Staff")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ToggleCategoryStatus(short id)
        {
            try
            {
                var result = await _categoryService.ToggleStatusAsync(id);

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
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }

        // ========================= HELPER ENDPOINTS =========================

        [HttpGet("{id}/can-delete")]
        [Authorize(Roles = "Staff,Admin")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDetailResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CanDeleteCategory(short id)
        {
            try
            {
                var result = await _categoryService.GetCategoryByIdAsync(id);

                if (result.Success && result.Data != null)
                {
                    var canDeleteInfo = new
                    {
                        CategoryId = result.Data.CategoryId,
                        CategoryName = result.Data.CategoryName,
                        CanDelete = result.Data.CanDelete,
                        NewsArticleCount = result.Data.NewsArticleCount,
                        SubCategoryCount = result.Data.SubCategoryCount,
                        Reason = result.Data.CanDelete
                            ? "Category can be deleted"
                            : "Cannot delete: " +
                              (result.Data.NewsArticleCount > 0 ? $"Has {result.Data.NewsArticleCount} news articles. " : "") +
                              (result.Data.SubCategoryCount > 0 ? $"Has {result.Data.SubCategoryCount} sub-categories." : "")
                    };

                    return Ok(ApiResponse<object>.SuccessResponse(
                        canDeleteInfo,
                        "Category delete check completed"
                    ));
                }

                return NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Internal server error occurred",
                    new List<string> { ex.Message }
                ));
            }
        }
    }
}