using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.Dto
{
    // ========================= REQUEST DTOs =========================
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Category description cannot exceed 500 characters")]
        public string? CategoryDescrip { get; set; }

        public short? ParentCategoryId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }

    public class UpdateCategoryRequest
    {
        [Required]
        public short CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Category description cannot exceed 500 characters")]
        public string? CategoryDescrip { get; set; }

        public short? ParentCategoryId { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }

    public class CategorySearchRequest
    {
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public short? ParentCategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ========================= RESPONSE DTOs =========================
    public class CategorySimpleResponse
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
    public class CategoryDetailResponse
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryDescrip { get; set; }
        public short? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool IsActive { get; set; }

        // Statistics
        public int NewsArticleCount { get; set; }
        public int SubCategoryCount { get; set; }
        public bool CanDelete { get; set; }

        // Related data
        public List<CategorySimpleResponse> SubCategories { get; set; } = new();
    }
    public class CategoryListResponse
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryDescrip { get; set; }
        public short? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool IsActive { get; set; }
        public int NewsArticleCount { get; set; }
        public int SubCategoryCount { get; set; }
        public bool CanDelete { get; set; }
    }

    /// <summary>
    /// DTO cho hierarchical tree view
    /// </summary>
    public class CategoryTreeResponse
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int NewsArticleCount { get; set; }
        public List<CategoryTreeResponse> Children { get; set; } = new();
    }

    /// <summary>
    /// Paginated response wrapper
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    /// <summary>
    /// Generic API Response wrapper
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}