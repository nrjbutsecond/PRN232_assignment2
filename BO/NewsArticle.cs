using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class NewsArticle
    {
        [Key]
        public int NewsArticleId { get; set; }
        public string NewsTitle { get; set; } = string.Empty;
        public string? Headline { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NewsContent { get; set; } = string.Empty;
        public string? NewsSource { get; set; }

        public short CategoryId { get; set; }

        public bool NewsStatus { get; set; } = true;

        public int CreatedById { get; set; }

        public int UpdatedById { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey(nameof(CreatedById))]
        public virtual SystemAccount CreatedBy { get; set; } = null!;
        public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();

    }
}