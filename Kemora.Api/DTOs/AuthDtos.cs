using System.ComponentModel.DataAnnotations;

namespace Kemora.Api.DTOs
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
    }
}