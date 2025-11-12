using BO;
using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class NewArticleRepository : INewArticleRepository
    {
        private readonly NewsArticleDAO _DAO;
        public NewArticleRepository (NewsArticleDAO dao)
        {
            _DAO = dao;
        }

        public Task<NewsArticle> AddAsync(NewsArticle newsArticle) => _DAO.AddAsync(newsArticle);
        

        public Task<int> CountByCategoryIdAsync(short categoryId) => _DAO.CountByCategoryIdAsync (categoryId);
     

        public Task<int> CountByCreatedByIdAsync(int createdById) =>_DAO.CountByCreatedByIdAsync (createdById);
     

        public Task DeleteAsync(int id) => _DAO.DeleteAsync(id);
      

        public Task<bool> ExistsAsync(int id) => _DAO.ExistsAsync(id);
    
        public Task<List<NewsArticle>> GetActiveNewsAsync() => _DAO.GetActiveNewsAsync();
   

        public Task<List<NewsArticle>> GetAllAsync() => _DAO.GetAllAsync();

        public Task<List<NewsArticle>> GetByCategoryIdAsync(short categoryId, bool activeOnly = false) => _DAO.GetByCategoryIdAsync(categoryId, activeOnly);
    

        public Task<List<NewsArticle>> GetByCreatedByIdAsync(int createdById) => _DAO.GetByCreatedByIdAsync(createdById);
   

        public Task<NewsArticle?> GetByIdAsync(int id, bool includeInactive = false) => _DAO.GetByIdAsync(id, includeInactive);
    
        public Task<bool> IsOwnerAsync(int newsArticleId, int userId) => _DAO.IsOwnerAsync(newsArticleId, userId);


        public Task<List<NewsArticle>> SearchAsync(string searchTerm, short? categoryId = null, bool? newsStatus = null, int? createdById = null, DateTime? fromDate = null, DateTime? toDate = null) => _DAO.SearchAsync(searchTerm, categoryId, newsStatus, createdById, fromDate, toDate);
    

        public Task<List<NewsArticle>> SearchByTagsAsync(List<string> tagNames) => _DAO.SearchByTagsAsync(tagNames);
    
        public Task SetStatusAsync(int id, bool status) => _DAO.SetStatusAsync(id, status);


        public Task ToggleStatusAsync(int id) => _DAO.ToggleStatusAsync(id);


        public Task UpdateAsync(NewsArticle newsArticle) => _DAO.UpdateAsync(newsArticle);
    
    }
}
