using Kemora.Domain.Entities;

namespace Kemora.Domain.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}