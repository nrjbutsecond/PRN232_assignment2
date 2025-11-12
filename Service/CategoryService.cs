using BO;
using BO.Dto;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // ========================= GET ALL =========================
        public async Task<ApiResponse<List<CategoryListResponse>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesAsync();

                var response = new List<CategoryListResponse>();
                foreach (var category in categories)
                {
                    var newsCount = await _categoryRepository.GetNewsArticleCountAsync(category.CategoryId);
                    var subCount = await _categoryRepository.GetSubCategoryCountAsync(category.CategoryId);

                    response.Add(new CategoryListResponse
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        CategoryDescrip = category.CategoryDescrip,
                        ParentCategoryId = category.ParentCategoryId,
                        ParentCategoryName = category.ParentCategory?.CategoryName,
                        IsActive = category.IsActive,
                        NewsArticleCount = newsCount,
                        SubCategoryCount = subCount,
                        CanDelete = newsCount == 0 && subCount == 0
                    });
                }

                return ApiResponse<List<CategoryListResponse>>.SuccessResponse(
                    response,
                    $"Retrieved {response.Count} categories successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategoryListResponse>>.ErrorResponse(
                    "Error retrieving categories",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= GET ACTIVE =========================
        public async Task<ApiResponse<List<CategorySimpleResponse>>> GetActiveCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetActiveCategoriesAsync();

                var response = categories.Select(c => new CategorySimpleResponse
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    IsActive = c.IsActive
                }).ToList();

                return ApiResponse<List<CategorySimpleResponse>>.SuccessResponse(
                    response,
                    $"Retrieved {response.Count} active categories"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategorySimpleResponse>>.ErrorResponse(
                    "Error retrieving active categories",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= GET BY ID =========================
        public async Task<ApiResponse<CategoryDetailResponse>> GetCategoryByIdAsync(short id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    return ApiResponse<CategoryDetailResponse>.ErrorResponse(
                        $"Category with ID {id} not found"
                    );
                }

                var newsCount = await _categoryRepository.GetNewsArticleCountAsync(category.CategoryId);
                var subCount = await _categoryRepository.GetSubCategoryCountAsync(category.CategoryId);
                var subCategories = await _categoryRepository.GetSubCategoriesAsync(category.CategoryId);

                var response = new CategoryDetailResponse
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    CategoryDescrip = category.CategoryDescrip,
                    ParentCategoryId = category.ParentCategoryId,
                    ParentCategoryName = category.ParentCategory?.CategoryName,
                    IsActive = category.IsActive,
                    NewsArticleCount = newsCount,
                    SubCategoryCount = subCount,
                    CanDelete = newsCount == 0 && subCount == 0,
                    SubCategories = subCategories.Select(sc => new CategorySimpleResponse
                    {
                        CategoryId = sc.CategoryId,
                        CategoryName = sc.CategoryName,
                        IsActive = sc.IsActive
                    }).ToList()
                };

                return ApiResponse<CategoryDetailResponse>.SuccessResponse(
                    response,
                    "Category retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryDetailResponse>.ErrorResponse(
                    "Error retrieving category",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= SEARCH =========================
        public async Task<ApiResponse<List<CategoryListResponse>>> SearchCategoriesAsync(string searchTerm)
        {
            try
            {
                var categories = await _categoryRepository.SearchCategoryAsync(searchTerm);

                var response = new List<CategoryListResponse>();
                foreach (var category in categories)
                {
                    var newsCount = await _categoryRepository.GetNewsArticleCountAsync(category.CategoryId);
                    var subCount = await _categoryRepository.GetSubCategoryCountAsync(category.CategoryId);

                    response.Add(new CategoryListResponse
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        CategoryDescrip = category.CategoryDescrip,
                        ParentCategoryId = category.ParentCategoryId,
                        ParentCategoryName = category.ParentCategory?.CategoryName,
                        IsActive = category.IsActive,
                        NewsArticleCount = newsCount,
                        SubCategoryCount = subCount,
                        CanDelete = newsCount == 0 && subCount == 0
                    });
                }

                return ApiResponse<List<CategoryListResponse>>.SuccessResponse(
                    response,
                    $"Found {response.Count} categories matching '{searchTerm}'"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategoryListResponse>>.ErrorResponse(
                    "Error searching categories",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= CREATE =========================
        public async Task<ApiResponse<CategoryDetailResponse>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            try
            {
                // Validate: Category Name 
                var exists = await _categoryRepository.CategoryNameExistsAsync(request.CategoryName);
                if (exists)
                {
                    return ApiResponse<CategoryDetailResponse>.ErrorResponse(
                        $"Category '{request.CategoryName}' already exists",
                        new List<string> { "Duplicate category name" }
                    );
                }

                // Validate: Parent Category 
                if (request.ParentCategoryId.HasValue)
                {
                    var parentExists = await _categoryRepository.GetCategoryByIdAsync(request.ParentCategoryId.Value);
                    if (parentExists == null)
                    {
                        return ApiResponse<CategoryDetailResponse>.ErrorResponse(
                            "Parent category not found",
                            new List<string> { "Invalid parent category ID" }
                        );
                    }
                }

                var category = new Category
                {
                    CategoryName = request.CategoryName.Trim(),
                    CategoryDescrip = request.CategoryDescrip?.Trim(),
                    ParentCategoryId = request.ParentCategoryId,
                    IsActive = request.IsActive
                };

                var createdCategory = await _categoryRepository.AddAsync(category);

                // Return response
                var response = new CategoryDetailResponse
                {
                    CategoryId = createdCategory.CategoryId,
                    CategoryName = createdCategory.CategoryName,
                    CategoryDescrip = createdCategory.CategoryDescrip,
                    ParentCategoryId = createdCategory.ParentCategoryId,
                    IsActive = createdCategory.IsActive,
                    NewsArticleCount = 0,
                    SubCategoryCount = 0,
                    CanDelete = true,
                    SubCategories = new List<CategorySimpleResponse>()
                };

                return ApiResponse<CategoryDetailResponse>.SuccessResponse(
                    response,
                    "Category created successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryDetailResponse>.ErrorResponse(
                    "Error creating category",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= UPDATE =========================
        public async Task<ApiResponse<CategoryDetailResponse>> UpdateCategoryAsync(UpdateCategoryRequest request)
        {
            try
            {
                // Check category
                var existingCategory = await _categoryRepository.GetCategoryByIdAsync(request.CategoryId);
                if (existingCategory == null)
                {
                    return ApiResponse<CategoryDetailResponse>.ErrorResponse(
                        $"Category with ID {request.CategoryId} not found"
                    );
                }

                // Validate: Category Name
                var nameExists = await _categoryRepository.CategoryNameExistsAsync(
                    request.CategoryName,
                    request.CategoryId
                );
                if (nameExists)
                {
                    return ApiResponse<CategoryDetailResponse>.ErrorResponse(
                        $"Category '{request.CategoryName}' already exists",
                        new List<string> { "Duplicate category name" }
                    );
                }

                // Validate: Parent Category 
                if (request.ParentCategoryId.HasValue)
                {
                    var isValidParent = await _categoryRepository.IsValidParentAsync(
                        request.CategoryId,
                        request.ParentCategoryId
                    );
                    if (!isValidParent)
                    {
                        return ApiResponse<CategoryDetailResponse>.ErrorResponse(
                            "Invalid parent category. Cannot create circular reference.",
                            new List<string> { "Circular reference detected" }
                        );
                    }
                }

                // Update category
                existingCategory.CategoryName = request.CategoryName.Trim();
                existingCategory.CategoryDescrip = request.CategoryDescrip?.Trim();
                existingCategory.ParentCategoryId = request.ParentCategoryId;
                existingCategory.IsActive = request.IsActive;

                await _categoryRepository.UpdateAsync(existingCategory);

                // Return updated data
                return await GetCategoryByIdAsync(request.CategoryId);
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryDetailResponse>.ErrorResponse(
                    "Error updating category",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= DELETE =========================
        public async Task<ApiResponse<bool>> DeleteCategoryAsync(short id)
        {
            try
            {
                // Check category 
                var category = await _categoryRepository.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        $"Category with ID {id} not found"
                    );
                }

                // Check NewsArticle
                var hasNews = await _categoryRepository.HasNewsArticlesAsync(id);
                if (hasNews)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Cannot delete category. It has associated news articles.",
                        new List<string> {
                            "Please remove or reassign all news articles before deleting this category."
                        }
                    );
                }

                // Check SubCategory 
                var hasSubs = await _categoryRepository.HasSubCategoriesAsync(id);
                if (hasSubs)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Cannot delete category. It has sub-categories.",
                        new List<string> {
                            "Please delete or reassign all sub-categories first."
                        }
                    );
                }

                await _categoryRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Category deleted successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error deleting category",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= TOGGLE STATUS =========================
        public async Task<ApiResponse<bool>> ToggleStatusAsync(short id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        $"Category with ID {id} not found"
                    );
                }

                await _categoryRepository.ToggleStatusAsync(id);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    $"Category status changed to {(category.IsActive ? "Inactive" : "Active")}"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error toggling category status",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= GET CATEGORY TREE =========================
        public async Task<ApiResponse<List<CategoryTreeResponse>>> GetCategoryTreeAsync()
        {
            try
            {
                var rootCategories = await _categoryRepository.GetRootCategoriesAsync();
                var tree = new List<CategoryTreeResponse>();

                foreach (var root in rootCategories)
                {
                    tree.Add(await BuildCategoryTreeAsync(root));
                }

                return ApiResponse<List<CategoryTreeResponse>>.SuccessResponse(
                    tree,
                    "Category tree retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategoryTreeResponse>>.ErrorResponse(
                    "Error building category tree",
                    new List<string> { ex.Message }
                );
            }
        }

        private async Task<CategoryTreeResponse> BuildCategoryTreeAsync(Category category)
        {
            var newsCount = await _categoryRepository.GetNewsArticleCountAsync(category.CategoryId);
            var subCategories = await _categoryRepository.GetSubCategoriesAsync(category.CategoryId);

            var node = new CategoryTreeResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                IsActive = category.IsActive,
                NewsArticleCount = newsCount,
                Children = new List<CategoryTreeResponse>()
            };

            foreach (var sub in subCategories)
            {
                node.Children.Add(await BuildCategoryTreeAsync(sub));
            }

            return node;
        }

        // ========================= GET SUB CATEGORIES =========================
        public async Task<ApiResponse<List<CategorySimpleResponse>>> GetSubCategoriesAsync(short parentId)
        {
            try
            {
                var subCategories = await _categoryRepository.GetSubCategoriesAsync(parentId);

                var response = subCategories.Select(c => new CategorySimpleResponse
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    IsActive = c.IsActive
                }).ToList();

                return ApiResponse<List<CategorySimpleResponse>>.SuccessResponse(
                    response,
                    $"Retrieved {response.Count} sub-categories"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategorySimpleResponse>>.ErrorResponse(
                    "Error retrieving sub-categories",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= GET PAGED =========================
        public async Task<ApiResponse<PagedResult<CategoryListResponse>>> GetPagedCategoriesAsync(CategorySearchRequest request)
        {
            try
            {
                var allCategories = string.IsNullOrWhiteSpace(request.SearchTerm)
                    ? await _categoryRepository.GetCategoriesAsync()
                    : await _categoryRepository.SearchCategoryAsync(request.SearchTerm);

                // Filter by IsActive
                if (request.IsActive.HasValue)
                {
                    allCategories = allCategories.Where(c => c.IsActive == request.IsActive.Value).ToList();
                }

                // Filter by ParentCategoryId
                if (request.ParentCategoryId.HasValue)
                {
                    allCategories = allCategories.Where(c => c.ParentCategoryId == request.ParentCategoryId.Value).ToList();
                }

                var totalCount = allCategories.Count;

                // Pagination
                var pagedCategories = allCategories
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var items = new List<CategoryListResponse>();
                foreach (var category in pagedCategories)
                {
                    var newsCount = await _categoryRepository.GetNewsArticleCountAsync(category.CategoryId);
                    var subCount = await _categoryRepository.GetSubCategoryCountAsync(category.CategoryId);

                    items.Add(new CategoryListResponse
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        CategoryDescrip = category.CategoryDescrip,
                        ParentCategoryId = category.ParentCategoryId,
                        ParentCategoryName = category.ParentCategory?.CategoryName,
                        IsActive = category.IsActive,
                        NewsArticleCount = newsCount,
                        SubCategoryCount = subCount,
                        CanDelete = newsCount == 0 && subCount == 0
                    });
                }

                var result = new PagedResult<CategoryListResponse>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return ApiResponse<PagedResult<CategoryListResponse>>.SuccessResponse(
                    result,
                    $"Retrieved page {request.PageNumber} of categories"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<CategoryListResponse>>.ErrorResponse(
                    "Error retrieving paged categories",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}