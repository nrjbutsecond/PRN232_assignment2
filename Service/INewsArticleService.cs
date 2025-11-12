using BO.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BO.Dto.NewsArticleDtos;

namespace Service
{
    public interface INewsArticleService
    {
        // ========================= PUBLIC (Xem News Active) =========================
        Task<ApiResponse<List<NewsArticlePublicResponse>>> GetActiveNewsArticlesAsync();
        Task<ApiResponse<NewsArticleDetailResponse>> GetNewsArticleByIdAsync(int id, int? currentUserId = null);

        // ========================= STAFF - CRUD =========================
        Task<ApiResponse<List<NewsArticleListResponse>>> GetAllNewsArticlesAsync();
        Task<ApiResponse<List<NewsArticleListResponse>>> GetMyNewsArticlesAsync(int staffId);
        Task<ApiResponse<List<NewsArticleListResponse>>> SearchNewsArticlesAsync(string searchTerm);
        Task<ApiResponse<NewsArticleDetailResponse>> CreateNewsArticleAsync(CreateNewsArticleRequest request, int staffId);
        Task<ApiResponse<NewsArticleDetailResponse>> UpdateNewsArticleAsync(UpdateNewsArticleRequest request, int staffId);
        Task<ApiResponse<bool>> DeleteNewsArticleAsync(int id, int staffId);

       
    }
    }

