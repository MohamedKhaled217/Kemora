using Kemora.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponseDto?> CreateReviewAsync(string userId, string userName, int placeId, CreateReviewDto dto);
        Task<PagedResult<ReviewResponseDto>> GetReviewsAsync(int placeId, int page, int pageSize);
        Task<bool> DeleteReviewAsync(int id, string userName, string userId);
    }
}
