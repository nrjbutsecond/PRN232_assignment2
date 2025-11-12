using BO;
using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class NewsTagRepository : INewsTagRepository
    {
        private readonly NewsTagDAO dao;
        public NewsTagRepository(NewsTagDAO dao)
        {
            this.dao = dao;
        }

        public Task AddTagsToNewsArticleAsync(int newsArticleId, List<int> tagIds) => dao.AddTagsToNewsArticleAsync(newsArticleId, tagIds);
        

        public Task<int> CountNewsArticlesByTagIdAsync(int tagId) => dao.CountNewsArticlesByTagIdAsync(tagId);
   

        public Task<List<Tag>> GetTagsByNewsArticleIdAsync(int newsArticleId) => dao.GetTagsByNewsArticleIdAsync(newsArticleId);


        public Task RemoveAllTagsFromNewsArticleAsync(int newsArticleId) => dao.RemoveAllTagsFromNewsArticleAsync(newsArticleId);

        public Task UpdateTagsForNewsArticleAsync(int newsArticleId, List<int> tagIds) => dao.UpdateTagsForNewsArticleAsync(newsArticleId, tagIds);
 
    }
}
