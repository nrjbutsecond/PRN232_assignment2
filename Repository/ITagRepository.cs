using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ITagRepository
    {
 
        Task<List<Tag>> GetAllAsync();   
        Task<Tag?> GetByIdAsync(int id);
        Task<Tag> GetOrCreateTagAsync(string tagName);
        Task<List<Tag>> SearchByNameAsync(string tagName);
    }
}

