using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Services
{
    /// <summary>
    /// Implements IProfileService in Infrastructure because it requires
    /// UserManager<ApplicationUser> which is an ASP.NET Identity (Infrastructure) concern.
    /// The Application layer interface (IProfileService) is still in Application — 
    /// this maintains the dependency inversion principle correctly.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ProfileDto?> GetMyProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return new ProfileDto
            {
                Id = user.Id, Email = user.Email ?? "", FullName = user.FullName, TotalPoints = user.TotalPoints
            };
        }

        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            if (dto.FullName != null) user.FullName = dto.FullName;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<PublicProfileDto?> GetPublicProfileAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            return new PublicProfileDto
            {
                UserId = user.Id, FullName = user.FullName ?? "", TotalPoints = user.TotalPoints
            };
        }
    }
}
