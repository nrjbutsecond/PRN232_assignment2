using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class SystemAccount
    {
        [Key]
        public int AccountId {  get; set; }

        public string AccountName {  get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public int AccountRole {  get; set; }

        public string AccountPassword {  get; set; } =string.Empty;

        public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    }
}
