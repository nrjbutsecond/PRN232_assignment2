using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Tag
    {
        [Key]
        public int TagId {  get; set; }
        public string TagName { get; set; } = string.Empty;

        public string? Note {  get; set; }

        public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();

    }
}
