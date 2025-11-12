using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface INewsTagRepository
    {

        Task<List<Tag>> GetTagsByNewsArticleIdAsync(int newsArticleId);
        Task AddTagsToNewsArticleAsync(int newsArticleId, List<int> tagIds);
        Task RemoveAllTagsFromNewsArticleAsync(int newsArticleId);
        Task UpdateTagsForNewsArticleAsync(int newsArticleId, List<int> tagIds);
        Task<int> CountNewsArticlesByTagIdAsync(int tagId);
    }
}
