using Kemora.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Application.Interfaces
{
    public interface IPostService
    {
        Task<PostListResponseDto> CreateAsync(string userId, CreatePostDto dto);
        Task<PagedResult<PostListResponseDto>> GetPostsAsync(int page, int pageSize);
        Task<PostDetailResponseDto?> GetPostAsync(int id);
        Task<PagedResult<PostListResponseDto>> GetMyPostsAsync(string userId, int page, int pageSize);
        Task<bool> UpdatePostAsync(int id, string userId, UpdatePostDto dto);
        Task<bool> DeletePostAsync(int id, string userId);
    }
}
