using System.ComponentModel.DataAnnotations;

namespace Kemora.Application.DTOs
{
    public class RegisterDto
    {
        [Required] public string FullName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }

    public class LoginDto
    {
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        [Required] public string Token { get; set; }
        [Required] public string RefreshToken { get; set; }
    }

    public class ForgotPasswordDto
    {
        [Required, EmailAddress] public string Email { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Token { get; set; }
        [Required] public string NewPassword { get; set; }
    }

    public class ConfirmEmailDto
    {
        [Required] public string UserId { get; set; }
        [Required] public string Token { get; set; }
    }
}
