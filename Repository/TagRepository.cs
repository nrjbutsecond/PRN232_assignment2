using BO;
using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly TagDAO _tagDao;
        public TagRepository(TagDAO tagDao)
        {
            _tagDao = tagDao;
        }

        public Task<List<Tag>> GetAllAsync() => _tagDao.GetAllAsync();
       

        public Task<Tag?> GetByIdAsync(int id) => _tagDao.GetByIdAsync(id);
       
        public Task<Tag> GetOrCreateTagAsync(string tagName) => _tagDao.GetOrCreateTagAsync(tagName);
   

        public Task<List<Tag>> SearchByNameAsync(string tagName) => _tagDao.SearchByNameAsync(tagName);
        
    }
}
