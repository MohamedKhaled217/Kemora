using Kemora.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Application.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponseDto?> CreateCommentAsync(int postId, string userId, CreateCommentDto dto);
        Task<PagedResult<CommentResponseDto>> GetCommentsAsync(int postId, int page, int pageSize);
        Task<bool> UpdateCommentAsync(int id, string userId, UpdateCommentDto dto);
        Task<bool> DeleteCommentAsync(int id, string userId);
    }
}
