using Kemora.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Application.Interfaces
{
    public interface IUserManagementService
    {
        Task<List<ProfileDto>> GetAllUsersAsync();
        Task<bool> AssignRoleAsync(string userId, string role);
        Task<bool> RemoveRoleAsync(string userId, string role);
    }
}
