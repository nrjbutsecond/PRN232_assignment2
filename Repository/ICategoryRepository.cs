using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<List<Category>> GetActiveCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(short id);
        Task<List<Category>> SearchCategoryAsync(string searchTerm);
        Task<Category> AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(short id);
        Task ToggleStatusAsync(short categoryId);
        Task SetActiveStatusAsync(short categoryId, bool isActive);
        Task<int> GetNewsArticleCountAsync(short categoryId);
        Task<int> GetSubCategoryCountAsync(short categoryId);
        Task<List<Category>> GetSubCategoriesAsync(short parentCategoryId);
        Task<List<Category>> GetRootCategoriesAsync();
        Task<bool> IsValidParentAsync(short categoryId, short? parentCategoryId);
        Task<bool> CategoryNameExistsAsync(string categoryName, short? excludeCategoryId = null);
        Task<bool> HasNewsArticlesAsync(short categoryId);
        Task<bool> HasSubCategoriesAsync(short categoryId);





    }
}
