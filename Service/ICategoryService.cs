using BO;
using BO.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ICategoryService
    {
        Task<ApiResponse<List<CategoryListResponse>>> GetAllCategoriesAsync();
        Task<ApiResponse<List<CategorySimpleResponse>>> GetActiveCategoriesAsync();
        Task<ApiResponse<CategoryDetailResponse>> GetCategoryByIdAsync(short id);
        Task<ApiResponse<List<CategoryListResponse>>> SearchCategoriesAsync(string searchTerm);
        Task<ApiResponse<CategoryDetailResponse>> CreateCategoryAsync(CreateCategoryRequest request);
        Task<ApiResponse<CategoryDetailResponse>> UpdateCategoryAsync(UpdateCategoryRequest request);
        Task<ApiResponse<bool>> DeleteCategoryAsync(short id);
        Task<ApiResponse<bool>> ToggleStatusAsync(short id);
        Task<ApiResponse<List<CategoryTreeResponse>>> GetCategoryTreeAsync();
        Task<ApiResponse<List<CategorySimpleResponse>>> GetSubCategoriesAsync(short parentId);
        Task<ApiResponse<PagedResult<CategoryListResponse>>> GetPagedCategoriesAsync(CategorySearchRequest request);
    }
}
