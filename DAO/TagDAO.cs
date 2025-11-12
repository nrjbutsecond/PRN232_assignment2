using BO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class TagDAO
    {
        private readonly FUNewsManagementDbContext _context;

        public TagDAO(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        // ========================= TAG CRUD =========================

        /// <summary>
        /// Lấy tất cả Tags
        /// </summary>
        public async Task<List<Tag>> GetAllAsync()
        {
            return await _context.Tags
                .OrderBy(t => t.TagName)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Lấy Tag by ID
        /// </summary>
        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TagId == id);
        }

        /// <summary>
        /// Tìm hoặc tạo Tag by name (case-insensitive)
        /// </summary>
        public async Task<Tag> GetOrCreateTagAsync(string tagName)
        {
            var normalizedName = tagName.Trim();

            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.TagName.ToLower() == normalizedName.ToLower());

            if (existingTag != null)
            {
                return existingTag;
            }

            // Create new tag
            var newTag = new Tag
            {
                TagName = normalizedName
            };

            _context.Tags.Add(newTag);
            await _context.SaveChangesAsync();

            return newTag;
        }

        /// <summary>
        /// Search tags by name
        /// </summary>
        public async Task<List<Tag>> SearchByNameAsync(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return new List<Tag>();
            }

            tagName = tagName.ToLower().Trim();

            return await _context.Tags
                .Where(t => t.TagName.ToLower().Contains(tagName))
                .OrderBy(t => t.TagName)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}