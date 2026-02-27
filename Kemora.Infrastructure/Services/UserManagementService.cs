using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<ProfileDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(u => new ProfileDto
            {
                Id = u.Id,
                Email = u.Email ?? "",
                FullName = u.FullName,
                TotalPoints = u.TotalPoints
            }).ToList();
        }

        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role)) return false;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }

        public async Task<bool> RemoveRoleAsync(string userId, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role)) return false;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            return result.Succeeded;
        }
    }
}
