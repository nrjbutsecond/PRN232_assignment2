using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class NewsTag
    {
        [Key, Column(Order = 0)]
        public int NewsArticleId { get; set; }

        [Key, Column(Order = 1)]
        public int TagId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(NewsArticleId))]
        public virtual NewsArticle NewsArticle { get; set; } = null!;

        [ForeignKey(nameof(TagId))]
        public virtual Tag Tag { get; set; } = null!;
    }
}
