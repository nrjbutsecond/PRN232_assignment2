using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Category
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public string? CategoryDescrip { get; set; }
        public short? ParentCategoryId { get; set; }

        public bool IsActive { get; set; } = true;



        [ForeignKey(nameof(ParentCategoryId))]
        public virtual Category? ParentCategory { get; set; }

        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    }
}

