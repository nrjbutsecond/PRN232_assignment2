using BO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class NewsArticleDAO
    {
        private readonly FUNewsManagementDbContext _context;

        public NewsArticleDAO(FUNewsManagementDbContext context)
        {
            _context = context;
        }


        public async Task<List<NewsArticle>> GetAllAsync()
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags) 
            .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<NewsArticle>> GetActiveNewsAsync()
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags) 
            .ThenInclude(nt => nt.Tag)
                .Where(n => n.NewsStatus == true && n.Category.IsActive == true)
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<NewsArticle?> GetByIdAsync(int id, bool includeInactive = false)
        {
            var query = _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
            .ThenInclude(nt => nt.Tag)
            .AsNoTracking()
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(n => n.NewsStatus == true);
            }

            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);
        }

        public async Task<List<NewsArticle>> GetByCreatedByIdAsync(int createdById)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags) 
            .ThenInclude(nt => nt.Tag)
                .Where(n => n.CreatedById == createdById)
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<NewsArticle>> GetByCategoryIdAsync(short categoryId, bool activeOnly = false)
        {
            var query = _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags) 
            .ThenInclude(nt => nt.Tag)
                .Where(n => n.CategoryId == categoryId);

            if (activeOnly)
            {
                query = query.Where(n => n.NewsStatus == true);
            }

            return await query
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<NewsArticle>> SearchAsync(
            string searchTerm,
            short? categoryId = null,
            bool? newsStatus = null,
            int? createdById = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags) 
            .ThenInclude(nt => nt.Tag)
                .AsQueryable();

            // Search by keyword
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(n =>
                    n.NewsTitle.ToLower().Contains(searchTerm) ||
                    n.NewsContent.ToLower().Contains(searchTerm) ||
                    (n.Headline != null && n.Headline.ToLower().Contains(searchTerm)) ||
                    n.NewsTags.Any(nt => nt.Tag.TagName.ToLower().Contains(searchTerm))
                );
            }

            if (categoryId.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryId.Value);
            }

            if (newsStatus.HasValue)
            {
                query = query.Where(n => n.NewsStatus == newsStatus.Value);
            }

            if (createdById.HasValue)
            {
                query = query.Where(n => n.CreatedById == createdById.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(n => n.CreatedDate <= endDate);
            }

            return await query
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<NewsArticle>> SearchByTagsAsync(List<string> tagNames)
        {
            if (tagNames == null || !tagNames.Any())
            {
                return new List<NewsArticle>();
            }

            var normalizedTags = tagNames.Select(t => t.ToLower().Trim()).ToList();

            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
            .ThenInclude(nt => nt.Tag)
                .Where(n => n.NewsTags
            .Any(nt => normalizedTags.Contains(nt.Tag.TagName.ToLower())))
                .OrderByDescending(n => n.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        // ========================= CREATE =========================

        /// <summary>
        /// Tạo NewsArticle mới (không bao gồm Tags - Tags được add riêng)
        /// </summary>
        public async Task<NewsArticle> AddAsync(NewsArticle newsArticle)
        {
            if (newsArticle == null)
            {
                throw new ArgumentNullException(nameof(newsArticle));
            }

            newsArticle.CreatedDate = DateTime.Now;
            newsArticle.ModifiedDate = null;

            _context.NewsArticles.Add(newsArticle);
            await _context.SaveChangesAsync();

            return newsArticle; 
        }

        // ========================= UPDATE =========================
        public async Task UpdateAsync(NewsArticle newsArticle)
        {
            if (newsArticle == null)
            {
                throw new ArgumentNullException(nameof(newsArticle));
            }

            var existingEntity = await _context.NewsArticles
                .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticle.NewsArticleId);

            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"NewsArticle with ID {newsArticle.NewsArticleId} not found.");
            }

            existingEntity.NewsTitle = newsArticle.NewsTitle;
            existingEntity.Headline = newsArticle.Headline;
            existingEntity.NewsContent = newsArticle.NewsContent;
            existingEntity.NewsSource = newsArticle.NewsSource;
            existingEntity.CategoryId = newsArticle.CategoryId;
            existingEntity.NewsStatus = newsArticle.NewsStatus;
            existingEntity.UpdatedById = newsArticle.UpdatedById;
            existingEntity.ModifiedDate = DateTime.Now;


            await _context.SaveChangesAsync();
        }

        // ========================= DELETE =========================

        /// <summary>
        /// Xóa NewsArticle (cascade delete Tags automatically)
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var newsArticle = await _context.NewsArticles
                .Include(n => n.NewsTags)
            .ThenInclude(nt => nt.Tag) 
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (newsArticle == null)
            {
                throw new KeyNotFoundException($"NewsArticle with ID {id} not found.");
            }

            _context.NewsArticles.Remove(newsArticle);
            await _context.SaveChangesAsync();
        }

        // ========================= STATUS =========================

        /// <summary>
        /// Toggle NewsStatus (Active/Inactive)
        /// </summary>
        public async Task ToggleStatusAsync(int id)
        {
            var newsArticle = await _context.NewsArticles
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (newsArticle == null)
            {
                throw new KeyNotFoundException($"NewsArticle with ID {id} not found.");
            }

            newsArticle.NewsStatus = !newsArticle.NewsStatus;
            newsArticle.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task SetStatusAsync(int id, bool status)
        {
            var newsArticle = await _context.NewsArticles
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (newsArticle == null)
            {
                throw new KeyNotFoundException($"NewsArticle with ID {id} not found.");
            }

            newsArticle.NewsStatus = status;
            newsArticle.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }



        // ========================= VALIDATION & HELPERS =========================
        public async Task<bool> IsOwnerAsync(int newsArticleId, int userId)
        {
            return await _context.NewsArticles
                .AnyAsync(n => n.NewsArticleId == newsArticleId && n.CreatedById == userId);
        }

        public async Task<int> CountByCreatedByIdAsync(int createdById)
        {
            return await _context.NewsArticles
                .CountAsync(n => n.CreatedById == createdById);
        }

        public async Task<int> CountByCategoryIdAsync(short categoryId)
        {
            return await _context.NewsArticles
                .CountAsync(n => n.CategoryId == categoryId);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.NewsArticles
                .AnyAsync(n => n.NewsArticleId == id);
        }

        
    }
}
    