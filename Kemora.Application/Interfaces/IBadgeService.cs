using Kemora.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Application.Interfaces
{
    public interface IBadgeService
    {
        Task<BadgeResponseDto> CreateBadgeAsync(CreateBadgeDto dto);
        Task<List<BadgeResponseDto>> GetAllBadgesAsync();
        Task<List<UserBadgeResponseDto>> GetMyBadgesAsync(string userId);
        Task<PointsSummaryDto> GetMyPointsAsync(string userId);
        Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int top);
    }
}
