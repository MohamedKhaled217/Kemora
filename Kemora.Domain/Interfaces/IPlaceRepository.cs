using Kemora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface IPlaceRepository : IRepository<Place>
    {
        Task<IEnumerable<Place>> GetFilteredAsync(string? query, int? governorateId, int? categoryId, int page, int size);
        Task<int> GetFilteredCountAsync(string? query, int? governorateId, int? categoryId);
        Task<Place?> GetWithDetailsAsync(int id);
        Task<List<Governorate>> GetAllGovernoratesAsync();
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<PlaceType>> GetAllPlaceTypesAsync();
        Task<bool> PlaceExistsAsync(int id);
        Task<PlaceType?> GetPlaceTypeAsync(int id);
        Task<Category?> GetCategoryAsync(int id);
        Task<Governorate?> GetGovernorateAsync(int id);
    }
}
