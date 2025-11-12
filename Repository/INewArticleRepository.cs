using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface INewArticleRepository
    {
        Task<List<NewsArticle>> GetAllAsync();
        Task<List<NewsArticle>> GetActiveNewsAsync();
        Task<NewsArticle?> GetByIdAsync(int id, bool includeInactive = false);
        Task<List<NewsArticle>> GetByCreatedByIdAsync(int createdById);
        Task<List<NewsArticle>> GetByCategoryIdAsync(short categoryId, bool activeOnly = false);
        Task<List<NewsArticle>> SearchAsync(
            string searchTerm,
            short? categoryId = null,
            bool? newsStatus = null,
            int? createdById = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);
        Task<List<NewsArticle>> SearchByTagsAsync(List<string> tagNames);
        Task<NewsArticle> AddAsync(NewsArticle newsArticle);
        Task UpdateAsync(NewsArticle newsArticle);
        Task DeleteAsync(int id);
        Task ToggleStatusAsync(int id);
        Task SetStatusAsync(int id, bool status);
        Task<bool> IsOwnerAsync(int newsArticleId, int userId);
        Task<int> CountByCreatedByIdAsync(int createdById);
        Task<int> CountByCategoryIdAsync(short categoryId);
        Task<bool> ExistsAsync(int id);
    }
}
