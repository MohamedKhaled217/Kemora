using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kemora.Api.Controllers
{
    /// <summary>
    /// Gamification endpoints: badges, points, and leaderboard.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [Authorize]
    public class BadgesController : ControllerBase
    {
        private readonly IBadgeService _badgeService;

        public BadgesController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        /// <summary>
        /// Create a new badge (Admin only).
        /// </summary>
        [HttpPost("admin/badges")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BadgeResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<BadgeResponseDto>> CreateBadge([FromBody] CreateBadgeDto dto)
        {
            var badge = await _badgeService.CreateBadgeAsync(dto);
            return Ok(badge);
        }

        /// <summary>
        /// Get all available badges.
        /// </summary>
        [HttpGet("badges")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<BadgeResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BadgeResponseDto>>> GetAllBadges()
        {
            return Ok(await _badgeService.GetAllBadgesAsync());
        }

        /// <summary>
        /// Get the authenticated user's earned badges.
        /// </summary>
        [HttpGet("my/badges")]
        [ProducesResponseType(typeof(List<UserBadgeResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserBadgeResponseDto>>> GetMyBadges()
        {
            return Ok(await _badgeService.GetMyBadgesAsync(GetUserId()));
        }

        /// <summary>
        /// Get the authenticated user's point summary.
        /// </summary>
        [HttpGet("my/points")]
        [ProducesResponseType(typeof(PointsSummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PointsSummaryDto>> GetMyPoints()
        {
            return Ok(await _badgeService.GetMyPointsAsync(GetUserId()));
        }

        /// <summary>
        /// Get the top users by points leaderboard.
        /// </summary>
        /// <param name="top">Number of top users to return (default: 10).</param>
        [HttpGet("leaderboard")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<LeaderboardEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<LeaderboardEntryDto>>> GetLeaderboard([FromQuery] int top = 10)
        {
            return Ok(await _badgeService.GetLeaderboardAsync(top));
        }
    }
}
