using Kemora.Api.DTOs;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kemora.Infrastructure.Data;
namespace Kemora.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto model)
        {
            // 1. Start a Transaction
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                    return BadRequest("Email already exists");

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    TotalPoints = 50
                };

                // 2. Save User (Database is modified here, but not "committed" permanently yet)
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded) return BadRequest(result.Errors);

                // 3. Generate Token (If this fails, we jump to 'catch')
                var token = _tokenService.CreateToken(user);

                // 4. Commit (Everything worked, so make the DB changes permanent)
                await transaction.CommitAsync();

                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Token = token
                };
            }
            catch (Exception ex)
            {
                // 5. Rollback (Something failed! Undo the user creation)
                await transaction.RollbackAsync();
                return StatusCode(500, "Registration failed: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("Invalid Email");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid Password");

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}