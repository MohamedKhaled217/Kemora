using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface IWikipediaService
    {
        Task<string?> GetImageUrlAsync(string title);
    }
}
