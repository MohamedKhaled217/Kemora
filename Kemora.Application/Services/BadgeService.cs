using AutoMapper;
using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Application.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly IBadgeRepository _badgeRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public BadgeService(IBadgeRepository badgeRepo, IUserRepository userRepo, IMapper mapper, IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _badgeRepo = badgeRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<BadgeResponseDto> CreateBadgeAsync(CreateBadgeDto dto)
        {
            var badge = new Badge { Name = dto.Name, Description = dto.Description, PointsReward = dto.PointsReward };
            await _badgeRepo.AddAsync(badge);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<BadgeResponseDto>(badge);
        }

        public async Task<List<BadgeResponseDto>> GetAllBadgesAsync()
        {
            var badges = await _badgeRepo.GetAllAsync();
            return _mapper.Map<List<BadgeResponseDto>>(badges);
        }

        public async Task<List<UserBadgeResponseDto>> GetMyBadgesAsync(string userId)
        {
            var ub = await _badgeRepo.GetUserBadgesAsync(userId);
            return _mapper.Map<List<UserBadgeResponseDto>>(ub);
        }

        public async Task<PointsSummaryDto> GetMyPointsAsync(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return new PointsSummaryDto();

            var history = await _userRepo.GetPointHistoryAsync(userId);
            var resultHistory = history.Select(h => new PointHistoryDto
            {
                PointsGained = h.PointsGained, GainedAt = h.GainedAt,
                SourcePlaceName = h.SourcePlace?.Name
            }).ToList();

            return new PointsSummaryDto { TotalPoints = user.TotalPoints, History = resultHistory };
        }

        public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int top)
        {
            var cacheKey = $"leaderboard_{top}";
            var cachedLeaderboard = _cacheService.Get<List<LeaderboardEntryDto>>(cacheKey);
            
            if (cachedLeaderboard != null)
                return cachedLeaderboard;

            var users = await _badgeRepo.GetLeaderboardAsync(top);
            var leaderboard = users.Select((u, i) => new LeaderboardEntryDto
            {
                Rank = i + 1, UserId = u.Id, FullName = u.FullName ?? "User", TotalPoints = u.TotalPoints
            }).ToList();

            _cacheService.Set(cacheKey, leaderboard, System.TimeSpan.FromMinutes(10));
            return leaderboard;
        }
    }
}
