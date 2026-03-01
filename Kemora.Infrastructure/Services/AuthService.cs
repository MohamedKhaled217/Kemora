using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using System.Security.Claims;
using Google.Apis.Auth;

namespace Kemora.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ApplicationDbContext context,
            IEmailService emailService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<(bool Succeeded, string Error, AuthResponseDto Data)> RegisterAsync(RegisterDto model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                    return (false, "Email already exists", null);

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Country = model.Country,
                    TotalPoints = 50
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)), null);

                // Assign roles
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count == 0)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
                user.RefreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                // Send email confirmation
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _emailService.SendEmailAsync(user.Email!, "Confirm your Kemora email",
                    $"Please confirm your email using this token: {emailToken}");

                var token = await _tokenService.CreateTokenAsync(user);
                await transaction.CommitAsync();

                var response = _mapper.Map<AuthResponseDto>(user);
                response.Token = token;

                return (true, null, response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Registration failed: " + ex.Message, null);
            }
        }

        public async Task<(bool Succeeded, string Error, AuthResponseDto Data)> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return (false, "Invalid Email", null);

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return (false, "Invalid Password", null);

            user.RefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var token = await _tokenService.CreateTokenAsync(user);

            var response = _mapper.Map<AuthResponseDto>(user);
            response.Token = token;

            return (true, null, response);
        }

        public async Task<(bool Succeeded, string Error, AuthResponseDto Data)> GoogleLoginAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings();
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                if (payload == null)
                    return (false, "Invalid Google token.", null);

                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        FullName = payload.Name,
                        EmailConfirmed = payload.EmailVerified,
                        TotalPoints = 50
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return (false, string.Join(", ", result.Errors.Select(e => e.Description)), null);

                    var admins = await _userManager.GetUsersInRoleAsync("Admin");
                    if (admins.Count == 0)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                }

                user.RefreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                var token = await _tokenService.CreateTokenAsync(user);
                var response = _mapper.Map<AuthResponseDto>(user);
                response.Token = token;

                return (true, null, response);
            }
            catch (InvalidJwtException)
            {
                return (false, "Invalid Google token.", null);
            }
            catch (Exception ex)
            {
                return (false, "Google login failed: " + ex.Message, null);
            }
        }

        public async Task<(bool Succeeded, string Error, AuthResponseDto Data)> RefreshTokenAsync(RefreshTokenRequestDto model)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == model.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return (false, "Invalid or expired refresh token", null);

            user.RefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var newToken = await _tokenService.CreateTokenAsync(user);
            var response = _mapper.Map<AuthResponseDto>(user);
            response.Token = newToken;

            return (true, null, response);
        }

        public async Task<(bool Succeeded, string Error)> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (false, "User not found");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            return (true, null!);
        }

        public async Task<(bool Succeeded, string Error)> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return (true, null!); // Don't reveal that user doesn't exist

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendEmailAsync(email, "Reset your Kemora password",
                $"Use this token to reset your password: {token}");

            return (true, null!);
        }

        public async Task<(bool Succeeded, string Error)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return (false, "User not found");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            return (true, null!);
        }

        public async Task<(bool Succeeded, string Error)> SendEmailConfirmationAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return (false, "User not found");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailService.SendEmailAsync(user.Email!, "Confirm your Kemora email",
                $"Please confirm your email using this token: {token}");

            return (true, null!);
        }
    }
}
