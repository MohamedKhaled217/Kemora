using Kemora.Api.DTOs;
using Kemora.Domain.Entities;
using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Kemora.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class BadgesController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        public BadgesController(ApplicationDbContext ctx) => _ctx = ctx;

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost("admin/badges")]
        public async Task<ActionResult<BadgeResponseDto>> CreateBadge([FromBody] CreateBadgeDto dto)
        {
            var badge = new Badge { Name = dto.Name, Description = dto.Description, PointsReward = dto.PointsReward };
            _ctx.Badges.Add(badge);
            await _ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllBadges), null,
                new BadgeResponseDto { BadgeID = badge.BadgeID, Name = badge.Name, Description = badge.Description, PointsReward = badge.PointsReward });
        }

        [HttpGet("badges")]
        [AllowAnonymous]
        public async Task<ActionResult<List<BadgeResponseDto>>> GetAllBadges()
        {
            return Ok(await _ctx.Badges.Select(b => new BadgeResponseDto
            {
                BadgeID = b.BadgeID, Name = b.Name, Description = b.Description, PointsReward = b.PointsReward
            }).ToListAsync());
        }

        [HttpGet("badges/my")]
        public async Task<ActionResult<List<UserBadgeResponseDto>>> GetMyBadges()
        {
            var userId = GetUserId();
            return Ok(await _ctx.UserBadges
                .Include(ub => ub.Badge)
                .Where(ub => ub.UserID == userId)
                .Select(ub => new UserBadgeResponseDto
                {
                    BadgeID = ub.BadgeID, BadgeName = ub.Badge.Name,
                    BadgeDescription = ub.Badge.Description, EarnedAt = ub.EarnedAt
                }).ToListAsync());
        }

        [HttpGet("points/my")]
        public async Task<ActionResult<PointsSummaryDto>> GetMyPoints()
        {
            var userId = GetUserId();
            var user = await _ctx.Users.OfType<ApplicationUser>().FirstAsync(u => u.Id == userId);
            var history = await _ctx.UserPoints
                .Include(up => up.SourcePlace)
                .Where(up => up.UserID == userId)
                .OrderByDescending(up => up.GainedAt)
                .Select(up => new PointHistoryDto
                {
                    PointsGained = up.PointsGained, GainedAt = up.GainedAt,
                    SourcePlaceName = up.SourcePlace != null ? up.SourcePlace.Name : null
                }).ToListAsync();

            return Ok(new PointsSummaryDto { TotalPoints = user.TotalPoints, History = history });
        }

        [HttpGet("leaderboard")]
        [AllowAnonymous]
        public async Task<ActionResult<List<LeaderboardEntryDto>>> GetLeaderboard([FromQuery] int top = 20)
        {
            var users = await _ctx.Users.OfType<ApplicationUser>()
                .OrderByDescending(u => u.TotalPoints)
                .Take(top)
                .ToListAsync();

            return Ok(users.Select((u, i) => new LeaderboardEntryDto
            {
                Rank = i + 1, UserId = u.Id, FullName = u.FullName, TotalPoints = u.TotalPoints
            }).ToList());
        }
    }
}
