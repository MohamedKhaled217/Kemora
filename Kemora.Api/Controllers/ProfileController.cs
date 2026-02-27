using Kemora.Api.DTOs;
using Kemora.Domain.Entities;
using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kemora.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>Get the current authenticated user's profile.</summary>
        [HttpGet]
        public async Task<ActionResult<ProfileResponseDto>> GetMyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            return Ok(new ProfileResponseDto
            {
                UserId        = user.Id,
                FullName      = user.FullName,
                Email         = user.Email ?? string.Empty,
                TotalPoints   = user.TotalPoints,
                PostCount     = await _context.Posts.CountAsync(p => p.UserID == user.Id),
                TripCount     = await _context.Trips.CountAsync(t => t.UserID == user.Id),
                BadgeCount    = await _context.UserBadges.CountAsync(ub => ub.UserID == user.Id),
                FavoriteCount = await _context.UserFavorites.CountAsync(uf => uf.UserID == user.Id)
            });
        }

        /// <summary>Update the current user's profile.</summary>
        [HttpPut]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            user.FullName = dto.FullName;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }

        /// <summary>Get a public profile for any user.</summary>
        [HttpGet("{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<PublicProfileDto>> GetPublicProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            return Ok(new PublicProfileDto
            {
                UserId     = user.Id,
                FullName   = user.FullName,
                TotalPoints = user.TotalPoints,
                BadgeCount = await _context.UserBadges.CountAsync(ub => ub.UserID == user.Id),
                PostCount  = await _context.Posts.CountAsync(p => p.UserID == user.Id)
            });
        }
    }
}
