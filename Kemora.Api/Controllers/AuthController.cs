using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace Kemora.Api.Controllers
{
    /// <summary>
    /// Handles user authentication: registration, login, token refresh, email confirmation, and password reset.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user account.
        /// </summary>
        /// <param name="model">Registration details including full name, email, and password.</param>
        /// <returns>Authentication tokens and user info.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var (succeeded, error, data) = await _authService.RegisterAsync(model);
            if (!succeeded) return BadRequest(error);
            return Ok(data);
        }

        /// <summary>
        /// Login with email and password.
        /// </summary>
        /// <param name="model">Login credentials.</param>
        /// <returns>JWT access token and refresh token.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var (succeeded, error, data) = await _authService.LoginAsync(model);
            if (!succeeded) return Unauthorized(error);
            return Ok(data);
        }

        /// <summary>
        /// Login using Google OAuth token.
        /// </summary>
        /// <param name="model">External login details including the provider and ID token.</param>
        /// <returns>JWT access token and refresh token.</returns>
        [HttpPost("google-login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GoogleLogin([FromBody] ExternalLoginDto model)
        {
            if (model.Provider?.ToUpper() != "GOOGLE")
                return BadRequest("Unsupported provider");

            var (succeeded, error, data) = await _authService.GoogleLoginAsync(model.IdToken);
            if (!succeeded) return Unauthorized(error);
            return Ok(data);
        }

        /// <summary>
        /// Refresh an expired JWT token using a valid refresh token.
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto model)
        {
            var (succeeded, error, data) = await _authService.RefreshTokenAsync(model);
            if (!succeeded) return Unauthorized(error);
            return Ok(data);
        }

        /// <summary>
        /// Confirm a user's email address using the confirmation token sent during registration.
        /// </summary>
        [HttpPost("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
        {
            var (succeeded, error) = await _authService.ConfirmEmailAsync(model.UserId, model.Token);
            if (!succeeded) return BadRequest(error);
            return Ok(new { message = "Email confirmed successfully." });
        }

        /// <summary>
        /// Request a password reset token. An email will be sent if the account exists.
        /// </summary>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var (succeeded, error) = await _authService.ForgotPasswordAsync(model.Email);
            return Ok(new { message = "If your email exists, a reset link has been sent." });
        }

        /// <summary>
        /// Reset a user's password using the token received via email.
        /// </summary>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var (succeeded, error) = await _authService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
            if (!succeeded) return BadRequest(error);
            return Ok(new { message = "Password reset successfully." });
        }

        /// <summary>
        /// Manually trigger an email confirmation for a user (Admin only).
        /// </summary>
        [HttpPost("admin/send-confirmation")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendConfirmation([FromQuery] string email)
        {
            var (succeeded, error) = await _authService.SendEmailConfirmationAsync(email);
            if (!succeeded) return BadRequest(error);
            return Ok(new { message = "Confirmation email sent." });
        }
    }
}