using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.Dto
{
    public class NewsArticleDtos
    {
        public class CreateNewsArticleRequest
        {
            public string NewsTitle { get; set; } = string.Empty;

            public string? Headline { get; set; }

            public string NewsContent { get; set; } = string.Empty;

            public string? NewsSource { get; set; }

            public short CategoryId { get; set; }

            public bool NewsStatus { get; set; } = true;

            public List<string> Tags { get; set; } = new();
        }


        public class UpdateNewsArticleRequest
        {
            public int NewsArticleId { get; set; }


            public string NewsTitle { get; set; } = string.Empty;

            public string? Headline { get; set; }

            public string NewsContent { get; set; } = string.Empty;

            public string? NewsSource { get; set; }

            public short CategoryId { get; set; }

            public bool NewsStatus { get; set; }
            public List<string> Tags { get; set; } = new();
        }

        /// <summary>
        /// DTO cho search/filter NewsArticle
        /// </summary>
        public class NewsArticleSearchRequest
        {
            public string? SearchTerm { get; set; }
            public short? CategoryId { get; set; }
            public bool? NewsStatus { get; set; }
            public int? CreatedById { get; set; } 
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public List<string>? Tags { get; set; } 
            public string? SortBy { get; set; } = "CreatedDate"; 
            public string? SortOrder { get; set; } = "desc"; 
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }

        // ========================= RESPONSE DTOs =========================
       


        public class NewsArticleListResponse
        {
            public int NewsArticleId { get; set; }
            public string NewsTitle { get; set; } = string.Empty;
            public string? Headline { get; set; }
            public string NewsContentPreview { get; set; } = string.Empty; 
            public string? NewsSource { get; set; }

            public short CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;

            public bool NewsStatus { get; set; }
            public string NewsStatusText => NewsStatus ? "Active" : "Inactive";

            public int CreatedById { get; set; }
            public string CreatedByName { get; set; } = string.Empty;
            public string CreatedByEmail { get; set; } = string.Empty;

            public DateTime CreatedDate { get; set; }
            public DateTime? ModifiedDate { get; set; }

            // Tags
            public List<TagResponse> Tags { get; set; } = new();
            public int TagCount => Tags.Count;

            // Permissions
            public bool CanEdit { get; set; }
            public bool CanDelete { get; set; }
        }

        public class NewsArticleDetailResponse
        {
            public int NewsArticleId { get; set; }
            public string NewsTitle { get; set; } = string.Empty;
            public string? Headline { get; set; }
            public string NewsContent { get; set; } = string.Empty;
            public string? NewsSource { get; set; }

            public short CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public string? CategoryDescription { get; set; }

            public bool NewsStatus { get; set; }
            public string NewsStatusText => NewsStatus ? "Active" : "Inactive";

            public int CreatedById { get; set; }
            public string CreatedByName { get; set; } = string.Empty;
            public string CreatedByEmail { get; set; } = string.Empty;
            public int CreatedByRole { get; set; }

            public int UpdatedById { get; set; }
            public string UpdatedByName { get; set; } = string.Empty;

            public DateTime CreatedDate { get; set; }
            public DateTime? ModifiedDate { get; set; }

            public List<TagResponse> Tags { get; set; } = new();

            public bool CanEdit { get; set; }
            public bool CanDelete { get; set; }
        }

        public class NewsArticlePublicResponse
        {
            public int NewsArticleId { get; set; }
            public string NewsTitle { get; set; } = string.Empty;
            public string? Headline { get; set; }
            public string NewsContent { get; set; } = string.Empty;
            public string? NewsSource { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public List<TagResponse> Tags { get; set; } = new();
        }

        // ========================= TAG DTOs =========================
        public class TagResponse
        {
            public int TagId { get; set; }
            public string TagName { get; set; } = string.Empty;
            public string? Note { get; set; }
        }
        public class TagRequest
        {
            public string TagName { get; set; } = string.Empty;

            public string? Note { get; set; }
        }
    }
}