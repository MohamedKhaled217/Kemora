using Kemora.Application.DTOs;

namespace Kemora.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string Error, AuthResponseDto Data)> RegisterAsync(RegisterDto model);
        Task<(bool Succeeded, string Error, AuthResponseDto Data)> LoginAsync(LoginDto model);
        Task<(bool Succeeded, string Error, AuthResponseDto Data)> GoogleLoginAsync(string idToken);
        Task<(bool Succeeded, string Error, AuthResponseDto Data)> RefreshTokenAsync(RefreshTokenRequestDto model);
        Task<(bool Succeeded, string Error)> ConfirmEmailAsync(string userId, string token);
        Task<(bool Succeeded, string Error)> ForgotPasswordAsync(string email);
        Task<(bool Succeeded, string Error)> ResetPasswordAsync(string email, string token, string newPassword);
        Task<(bool Succeeded, string Error)> SendEmailConfirmationAsync(string email);
    }
}
