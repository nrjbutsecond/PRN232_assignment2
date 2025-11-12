using BO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class CategoryDAO
    {
        private readonly FUNewsManagementDbContext _context;

        public CategoryDAO(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.CategoryName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(short id)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<List<Category>> SearchCategoryAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetCategoriesAsync();
            }

            searchTerm = searchTerm.ToLower().Trim();

            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Where(c =>
                    c.CategoryName.ToLower().Contains(searchTerm) ||
                    (c.CategoryDescrip != null && c.CategoryDescrip.ToLower().Contains(searchTerm))
                )
                .OrderBy(c => c.CategoryName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category> AddAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var exists = await _context.Categories
                .AnyAsync(c => c.CategoryName.ToLower() == category.CategoryName.ToLower());

            if (exists)
            {
                throw new InvalidOperationException($"Category '{category.CategoryName}' already exists.");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var exists = await _context.Categories
                .AnyAsync(c =>
                    c.CategoryName.ToLower() == category.CategoryName.ToLower() &&
                    c.CategoryId != category.CategoryId
                );

            if (exists)
            {
                throw new InvalidOperationException($"Category '{category.CategoryName}' already exists.");
            }

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)  
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            bool hasNewsArticles = await HasNewsArticlesAsync(id);
            if (hasNewsArticles)
            {
                throw new InvalidOperationException(
                    "Cannot delete category. It has associated news articles. " +
                    "Please remove or reassign all news articles before deleting this category."
                );
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasNewsArticlesAsync(short categoryId)
        {
            return await _context.NewsArticles
                .AnyAsync(n => n.CategoryId == categoryId);
        }

        public async Task ToggleStatusAsync(short categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }

            category.IsActive = !category.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task SetActiveStatusAsync(short categoryId, bool isActive)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }

            category.IsActive = isActive;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CategoryNameExistsAsync(string categoryName, short? excludeCategoryId = null)
        {
            var query = _context.Categories
                .Where(c => c.CategoryName.ToLower() == categoryName.ToLower());

            if (excludeCategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId != excludeCategoryId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> GetNewsArticleCountAsync(short categoryId)
        {
            return await _context.NewsArticles
                .CountAsync(n => n.CategoryId == categoryId);
        }

        public async Task<int> GetSubCategoryCountAsync(short categoryId)
        {
            return await _context.Categories
                .CountAsync(c => c.ParentCategoryId == categoryId);
        }

        public async Task<List<Category>> GetSubCategoriesAsync(short parentCategoryId)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .OrderBy(c => c.CategoryName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Category>> GetRootCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Where(c => c.ParentCategoryId == null)
                .OrderBy(c => c.CategoryName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> IsValidParentAsync(short categoryId, short? parentCategoryId)
        {
            if (parentCategoryId == null)
                return true;

            if (categoryId == parentCategoryId)
                return false; 

            var parent = await GetCategoryByIdAsync(parentCategoryId.Value);
            while (parent?.ParentCategoryId != null)
            {
                if (parent.ParentCategoryId == categoryId)
                    return false; 

                parent = await GetCategoryByIdAsync(parent.ParentCategoryId.Value);
            }

            return true;
        }

        public async Task<bool> HasSubCategoriesAsync(short categoryId)
        {
            return await _context.Categories
                .AnyAsync(c => c.ParentCategoryId == categoryId);
        }
    }
}