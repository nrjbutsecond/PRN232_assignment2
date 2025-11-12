using BO;
using BO.Dto;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BO.Dto.NewsArticleDtos;

namespace Service
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewArticleRepository _newsArticleRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INewsTagRepository _newsTagRepository;


        public NewsArticleService(
            INewArticleRepository newsArticleRepository,
            ITagRepository tagRepository,
            ICategoryRepository categoryRepository,
            INewsTagRepository newsTagRepository)
        {
            _newsArticleRepository = newsArticleRepository;
            _tagRepository = tagRepository;
            _categoryRepository = categoryRepository;
            _newsTagRepository = newsTagRepository;

        }




        // ========================= PUBLIC =========================

        /// <summary>
        /// Lấy tất cả News Active (cho Public xem)
        /// </summary>
        public async Task<ApiResponse<List<NewsArticlePublicResponse>>> GetActiveNewsArticlesAsync()
        {
            try
            {
                var newsArticles = await _newsArticleRepository.GetActiveNewsAsync();

                var response = newsArticles.Select(n => new NewsArticlePublicResponse
                {
                    NewsArticleId = n.NewsArticleId,
                    NewsTitle = n.NewsTitle,
                    Headline = n.Headline,
                    NewsContent = n.NewsContent,
                    NewsSource = n.NewsSource,
                    CategoryName = n.Category.CategoryName,
                    CreatedDate = n.CreatedDate,
                    Tags = n.NewsTags.Select(nt => new TagResponse
                    {
                        TagId = nt.Tag.TagId,
                        TagName = nt.Tag.TagName,
                        Note = nt.Tag.Note
                    }).ToList()
                }).ToList();

                return ApiResponse<List<NewsArticlePublicResponse>>.SuccessResponse(
                    response,
                    $"Retrieved {response.Count} active news articles"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<NewsArticlePublicResponse>>.ErrorResponse(
                    "Error retrieving active news articles",
                    new List<string> { ex.Message }
                );
            }
        }

        /// <summary>
        /// Lấy chi tiết News (Public xem Active, Staff xem cả Inactive của mình)
        /// </summary>
        public async Task<ApiResponse<NewsArticleDetailResponse>> GetNewsArticleByIdAsync(int id, int? currentUserId = null)
        {
            try
            {
                var newsArticle = await _newsArticleRepository.GetByIdAsync(id, includeInactive: true);

                if (newsArticle == null)
                {
                    return ApiResponse<NewsArticleDetailResponse>.ErrorResponse(
                        $"News article with ID {id} not found"
                    );
                }

                // Check permission: Inactive News chỉ owner mới xem được
                if (!newsArticle.NewsStatus && currentUserId.HasValue)
                {
                    if (newsArticle.CreatedById != currentUserId.Value)
                    {
                        return ApiResponse<NewsArticleDetailResponse>.ErrorResponse(
                            "You don't have permission to view this news article"
                        );
                    }
                }

                var response = MapToDetailResponse(newsArticle, currentUserId);

                return ApiResponse<NewsArticleDetailResponse>.SuccessResponse(
                    response,
                    "News article retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<NewsArticleDetailResponse>.ErrorResponse(
                    "Error retrieving news article",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= STAFF - GET ALL =========================

        public async Task<ApiResponse<List<NewsArticleListResponse>>> GetAllNewsArticlesAsync()
        {
            try
            {
                var newsArticles = await _newsArticleRepository.GetAllAsync();

                var response = newsArticles.Select(n => MapToListResponse(n)).ToList();

                return ApiResponse<List<NewsArticleListResponse>>.SuccessResponse(
                    response,
                    $"Retrieved {response.Count} news articles"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<NewsArticleListResponse>>.ErrorResponse(
                    "Error retrieving news articles",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= STAFF - VIEW HISTORY (My News) =========================

        public async Task<ApiResponse<List<NewsArticleListResponse>>> GetMyNewsArticlesAsync(int staffId)
        {
            try
            {
                var newsArticles = await _newsArticleRepository.GetByCreatedByIdAsync(staffId);

                var response = newsArticles.Select(n => MapToListResponse(n, staffId)).ToList();

                return ApiResponse<List<NewsArticleListResponse>>.SuccessResponse(
                    response,
                    $"Retrieved {response.Count} news articles created by you"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<NewsArticleListResponse>>.ErrorResponse(
                    "Error retrieving your news articles",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= STAFF - SEARCH =========================

        public async Task<ApiResponse<List<NewsArticleListResponse>>> SearchNewsArticlesAsync(string searchTerm)
        {
            try
            {
                var newsArticles = await _newsArticleRepository.SearchAsync(searchTerm);

                var response = newsArticles.Select(n => MapToListResponse(n)).ToList();

                return ApiResponse<List<NewsArticleListResponse>>.SuccessResponse(
                    response,
                    $"Found {response.Count} news articles"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<NewsArticleListResponse>>.ErrorResponse(
                    "Error searching news articles",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= STAFF - CREATE =========================

        public async Task<ApiResponse<NewsArticleDetailResponse>> CreateNewsArticleAsync(
            CreateNewsArticleRequest request,
            int staffId)
        {
            try
            {
                // Validate Category
                var category = await _categoryRepository.GetCategoryByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return ApiResponse<NewsArticleDetailResponse>.ErrorResponse(
                        "Category not found"
                    );
                }

                if (!category.IsActive)
                {
                    return ApiResponse<NewsArticleDetailResponse>.ErrorResponse(
                        "Cannot create news in inactive category"
                    );
                }

                // Validate Tags
                if (request.Tags.Count > 10)
                {
                    return ApiResponse<NewsArticleDetailResponse>.ErrorResponse(
                        "Maximum 10 tags allowed"
                    );
                }

                // Create NewsArticle
                var newsArticle = new NewsArticle
                {
                    NewsTitle = request.NewsTitle.Trim(),
                    Headline = request.Headline?.Trim(),
                    NewsContent = request.NewsContent.Trim(),
                    NewsSource = request.NewsSource?.Trim(),
                    CategoryId = request.CategoryId,
                    NewsStatus = request.NewsStatus,
                    CreatedById = staffId,
                    UpdatedById = staffId
                };

                var createdNews = await _newsArticleRepository.AddAsync(newsArticle);

                // Add Tags
                if (request.Tags.Any())
                {
                    var tagIds = new List<int>();
                    foreach (var tagName in request.Tags)
                    {
                        var tag = await _tagRepository.GetOrCreateTagAsync(tagName);
                        tagIds.Add(tag.TagId);
                    }

                    await _newsTagRepository.AddTagsToNewsArticleAsync(createdNews.NewsArticleId, tagIds);
                }

                // Reload with full data
                // var fullNews = await _newsArticleRepository.GetByIdAsync(createdNews.NewsArticleId, true);
                var tags = request.Tags.Any()
    ? await _newsTagRepository.GetTagsByNewsArticleIdAsync(createdNews.NewsArticleId)
    : new List<Tag>();

                var response = new NewsArticleDetailResponse
                {
                    NewsArticleId = createdNews.NewsArticleId,
                    NewsTitle = createdNews.NewsTitle,
                    Headline = createdNews.Headline,
                    NewsContent = createdNews.NewsContent,
                    NewsSource = createdNews.NewsSource,
                    CategoryId = request.CategoryId,
                    CategoryName = category.CategoryName,
                    CategoryDescription = category.CategoryDescrip,
                    NewsStatus = createdNews.NewsStatus,
                    CreatedById = staffId,
                    CreatedByName = "", // Get from current user if needed
                    CreatedByEmail = "",
                    CreatedByRole = 1,
                    UpdatedById = staffId,
                    UpdatedByName = "",
                    CreatedDate = createdNews.CreatedDate,
                    ModifiedDate = null,
                    Tags = tags.Select(t => new TagResponse
                    {
                        TagId = t.TagId,
                        TagName = t.TagName,
                        Note = t.Note
                    }).ToList(),
                    CanEdit = true,
                    CanDelete = true
                };

                return ApiResponse<NewsArticleDetailResponse>.SuccessResponse(
                    response,
                    "News article created successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<NewsArticleDetailResponse>.ErrorResponse(
                    "Error creating news article",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= STAFF - UPDATE =========================

        public async Task<ApiResponse<NewsArticleDetailResponse>> UpdateNewsArticleAsync(
     UpdateNewsArticleRequest request,
     int staffId)
        {
            try
            {
                // 1️⃣ Kiểm tra tồn tại bài viết
                var existingNews = await _newsArticleRepository.GetByIdAsync(request.NewsArticleId, true);
                if (existingNews == null)
                {
                    return ApiResponse<NewsArticleDetailResponse>.ErrorResponse("News article not found");
                }

                // 2️⃣ Kiểm tra quyền sở hữu
                if (existingNews.CreatedById != staffId)
                {
                    return ApiResponse<NewsArticleDetailResponse>.ErrorResponse(
                        "You can only update your own news articles"
                    );
                }

                // 3️⃣ Validate Category
                var category = await _categoryRepository.GetCategoryByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return ApiResponse<NewsArticleDetailResponse>.ErrorResponse("Category not found");
                }

                if (request.Tags.Count > 10)
                {
                    return ApiResponse<NewsArticleDetailResponse>.ErrorResponse("Maximum 10 tags allowed");
                }

                existingNews.NewsTitle = request.NewsTitle.Trim();
                existingNews.Headline = request.Headline?.Trim();
                existingNews.NewsContent = request.NewsContent.Trim();
                existingNews.NewsSource = request.NewsSource?.Trim();
                existingNews.CategoryId = request.CategoryId;
                existingNews.NewsStatus = request.NewsStatus;
                existingNews.UpdatedById = staffId;
                existingNews.ModifiedDate = DateTime.UtcNow;

                await _newsArticleRepository.UpdateAsync(existingNews);

                var tagIds = new List<int>();
                foreach (var tagName in request.Tags)
                {
                    var tag = await _tagRepository.GetOrCreateTagAsync(tagName);
                    tagIds.Add(tag.TagId);
                }
                await _newsTagRepository.UpdateTagsForNewsArticleAsync(request.NewsArticleId, tagIds);

                var tags = request.Tags.Any()
                    ? await _newsTagRepository.GetTagsByNewsArticleIdAsync(existingNews.NewsArticleId)
                    : new List<Tag>();

                var response = new NewsArticleDetailResponse
                {
                    NewsArticleId = existingNews.NewsArticleId,
                    NewsTitle = existingNews.NewsTitle,
                    Headline = existingNews.Headline,
                    NewsContent = existingNews.NewsContent,
                    NewsSource = existingNews.NewsSource,
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    CategoryDescription = category.CategoryDescrip,
                    NewsStatus = existingNews.NewsStatus,
                    CreatedById = existingNews.CreatedById,
                    CreatedByName = "", 
                    CreatedByEmail = "",
                    CreatedByRole = 1,  // staff
                    UpdatedById = staffId,
                    UpdatedByName = "",
                    CreatedDate = existingNews.CreatedDate,
                    ModifiedDate = existingNews.ModifiedDate,
                    Tags = tags.Select(t => new TagResponse
                    {
                        TagId = t.TagId,
                        TagName = t.TagName,
                        Note = t.Note
                    }).ToList(),
                    CanEdit = true,
                    CanDelete = true
                };

                return ApiResponse<NewsArticleDetailResponse>.SuccessResponse(
                    response,
                    "News article updated successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<NewsArticleDetailResponse>.ErrorResponse(
                    "Error updating news article",
                    new List<string> { ex.Message }
                );
            }
        }

        // ========================= STAFF - DELETE =========================

        public async Task<ApiResponse<bool>> DeleteNewsArticleAsync(int id, int staffId)
        {
            try
            {
                var newsArticle = await _newsArticleRepository.GetByIdAsync(id, true);

                if (newsArticle == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "News article not found"
                    );
                }

                // Check ownership
                if (newsArticle.CreatedById != staffId)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "You can only delete your own news articles"
                    );
                }

                // Delete (NewsTags will cascade delete automatically)
                await _newsArticleRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "News article deleted successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error deleting news article",
                    new List<string> { ex.Message }
                );
            }
        }

       
        // ========================= HELPER METHODS =========================

        private NewsArticleListResponse MapToListResponse(NewsArticle n, int? currentUserId = null)
        {
            var contentPreview = n.NewsContent.Length > 200
                ? n.NewsContent.Substring(0, 200) + "..."
                : n.NewsContent;

            bool canEdit = currentUserId.HasValue && n.CreatedById == currentUserId.Value;
            bool canDelete = currentUserId.HasValue && n.CreatedById == currentUserId.Value;

            return new NewsArticleListResponse
            {
                NewsArticleId = n.NewsArticleId,
                NewsTitle = n.NewsTitle,
                Headline = n.Headline,
                NewsContentPreview = contentPreview,
                NewsSource = n.NewsSource,
                CategoryId = n.CategoryId,
                CategoryName = n.Category.CategoryName,
                NewsStatus = n.NewsStatus,
                CreatedById = n.CreatedById,
                CreatedByName = n.CreatedBy.AccountName,
                CreatedByEmail = n.CreatedBy.AccountEmail,
                CreatedDate = n.CreatedDate,
                ModifiedDate = n.ModifiedDate,
                Tags = n.NewsTags.Select(nt => new TagResponse
                {
                    TagId = nt.Tag.TagId,
                    TagName = nt.Tag.TagName,
                    Note = nt.Tag.Note
                }).ToList(),
                CanEdit = canEdit,
                CanDelete = canDelete
            };
        }

        private NewsArticleDetailResponse MapToDetailResponse(NewsArticle n, int? currentUserId = null)
        {
            bool canEdit = currentUserId.HasValue && n.CreatedById == currentUserId.Value;
            bool canDelete = currentUserId.HasValue && n.CreatedById == currentUserId.Value;

            return new NewsArticleDetailResponse
            {
                NewsArticleId = n.NewsArticleId,
                NewsTitle = n.NewsTitle,
                Headline = n.Headline,
                NewsContent = n.NewsContent,
                NewsSource = n.NewsSource,
                CategoryId = n.CategoryId,
                CategoryName = n.Category.CategoryName,
                CategoryDescription = n.Category.CategoryDescrip,
                NewsStatus = n.NewsStatus,
                CreatedById = n.CreatedById,
                CreatedByName = n.CreatedBy.AccountName,
                CreatedByEmail = n.CreatedBy.AccountEmail,
                CreatedByRole = n.CreatedBy.AccountRole,
                UpdatedById = n.UpdatedById,
                UpdatedByName = n.CreatedBy.AccountName,
                CreatedDate = n.CreatedDate,
                ModifiedDate = n.ModifiedDate,
                Tags = n.NewsTags.Select(nt => new TagResponse
                {
                    TagId = nt.Tag.TagId,
                    TagName = nt.Tag.TagName,
                    Note = nt.Tag.Note
                }).ToList(),
                CanEdit = canEdit,
                CanDelete = canDelete
            };
        }
    }
}