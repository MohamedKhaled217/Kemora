using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Api.Controllers
{
    /// <summary>
    /// Admin user management: list users, assign and remove roles.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Get all registered users (Admin only).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProfileDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProfileDto>>> GetAllUsers()
        {
            return Ok(await _userManagementService.GetAllUsersAsync());
        }

        /// <summary>
        /// Assign a role to a user (e.g., "Admin", "User").
        /// </summary>
        /// <param name="userId">Target user ID.</param>
        /// <param name="role">Role name to assign.</param>
        [HttpPost("{userId}/roles/{role}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            if (await _userManagementService.AssignRoleAsync(userId, role)) return NoContent();
            return NotFound();
        }

        /// <summary>
        /// Remove a role from a user.
        /// </summary>
        /// <param name="userId">Target user ID.</param>
        /// <param name="role">Role name to remove.</param>
        [HttpDelete("{userId}/roles/{role}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            if (await _userManagementService.RemoveRoleAsync(userId, role)) return NoContent();
            return NotFound();
        }
    }
}
