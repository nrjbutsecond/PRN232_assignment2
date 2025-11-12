using BO.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BO.Dto.NewsArticleDtos;

namespace Service
{
    public interface ITagService
    {
        Task<ApiResponse<List<TagResponse>>> GetAllTagsAsync();

    }
}
