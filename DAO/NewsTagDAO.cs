using BO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class NewsTagDAO
    {
        private readonly FUNewsManagementDbContext _context;
        public NewsTagDAO( FUNewsManagementDbContext context)
        {
            _context = context;
        }

  
        public async Task<List<Tag>> GetTagsByNewsArticleIdAsync(int newsArticleId)
        {
            return await _context.NewsTags
                .Where(nt => nt.NewsArticleId == newsArticleId)
                .Select(nt => nt.Tag)
                .OrderBy(t => t.TagName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddTagsToNewsArticleAsync(int newsArticleId, List<int> tagIds)
        {
            var newsTags = tagIds.Select(tagId => new NewsTag
            {
                NewsArticleId = newsArticleId,
                TagId = tagId
            }).ToList();

            _context.NewsTags.AddRange(newsTags);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAllTagsFromNewsArticleAsync(int newsArticleId)
        {
            var newsTags = await _context.NewsTags
                .Where(nt => nt.NewsArticleId == newsArticleId)
                .ToListAsync();

            if (newsTags.Any())
            {
                _context.NewsTags.RemoveRange(newsTags);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTagsForNewsArticleAsync(int newsArticleId, List<int> tagIds)
        {
            await RemoveAllTagsFromNewsArticleAsync(newsArticleId);

            if (tagIds.Any())
            {
                await AddTagsToNewsArticleAsync(newsArticleId, tagIds);
            }
        }

  
        public async Task<int> CountNewsArticlesByTagIdAsync(int tagId)
        {
            return await _context.NewsTags
                .CountAsync(nt => nt.TagId == tagId);
        }
    }
}
