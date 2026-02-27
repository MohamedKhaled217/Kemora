using Kemora.Domain.Entities;

namespace Kemora.Domain.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
    }
}