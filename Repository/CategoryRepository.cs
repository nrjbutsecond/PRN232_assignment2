using BO;
using DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public  class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryRepository(CategoryDAO categoryDAO) { 
        _categoryDAO = categoryDAO;
        
        }

        public Task<Category> AddAsync(Category category) => _categoryDAO.AddAsync(category);
        

        public Task DeleteAsync(short id) => _categoryDAO.DeleteAsync(id);
      

        public Task<List<Category>> GetActiveCategoriesAsync() => _categoryDAO.GetActiveCategoriesAsync();
      

        public Task<List<Category>> GetCategoriesAsync() => _categoryDAO.GetCategoriesAsync();
       

        public Task<Category?> GetCategoryByIdAsync(short id) => _categoryDAO.GetCategoryByIdAsync(id);
        

        public Task<int> GetNewsArticleCountAsync(short categoryId) => _categoryDAO.GetNewsArticleCountAsync(categoryId);
       

        public Task<List<Category>> SearchCategoryAsync(string searchTerm) => _categoryDAO.SearchCategoryAsync(searchTerm);
        

        public Task SetActiveStatusAsync(short categoryId, bool isActive) => _categoryDAO.SetActiveStatusAsync(categoryId, isActive);
       
        public Task ToggleStatusAsync(short categoryId) => _categoryDAO.ToggleStatusAsync(categoryId);
  

        public Task UpdateAsync(Category category) =>_categoryDAO.UpdateAsync(category);

        public async Task<bool> HasNewsArticlesAsync(short categoryId)
        {
            return await _categoryDAO.HasNewsArticlesAsync(categoryId);
        }

        public async Task<bool> HasSubCategoriesAsync(short categoryId) =>await _categoryDAO.HasSubCategoriesAsync(categoryId);
        

        public async Task<bool> CategoryNameExistsAsync(string categoryName, short? excludeCategoryId = null)
        {
            return await _categoryDAO.CategoryNameExistsAsync(categoryName, excludeCategoryId);
        }

        public async Task<int> GetSubCategoryCountAsync(short categoryId)
        {
            return await _categoryDAO.GetSubCategoryCountAsync(categoryId);
        }

        public async Task<List<Category>> GetSubCategoriesAsync(short parentCategoryId)
        {
            return await _categoryDAO.GetSubCategoriesAsync(parentCategoryId);
        }

        public async Task<List<Category>> GetRootCategoriesAsync()
        {
            return await _categoryDAO.GetRootCategoriesAsync();
        }

        public async Task<bool> IsValidParentAsync(short categoryId, short? parentCategoryId)
        {
            return await _categoryDAO.IsValidParentAsync(categoryId, parentCategoryId);
        }

    }
}
