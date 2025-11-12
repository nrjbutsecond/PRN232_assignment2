using BO.Dto;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BO.Dto.NewsArticleDtos;

namespace Service
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly INewsTagRepository _newsTagRepository;

        public TagService(
            ITagRepository tagRepository,
            INewsTagRepository newsTagRepository)
        {
            _tagRepository = tagRepository;
            _newsTagRepository = newsTagRepository;
        }

        public async Task<ApiResponse<List<NewsArticleDtos.TagResponse>>> GetAllTagsAsync()
        {
            try
            {
                var tags = await _tagRepository.GetAllAsync();

                var response = tags.Select(t => new TagResponse
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                    Note = t.Note
                }).ToList();

                return ApiResponse<List<TagResponse>>.SuccessResponse(
                    response,
                    $"Retrieved {response.Count} tags"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<TagResponse>>.ErrorResponse(
                    "Error retrieving tags",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}