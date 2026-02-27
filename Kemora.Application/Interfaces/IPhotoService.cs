using Kemora.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Application.Interfaces
{
    public interface IPhotoService
    {
        Task<PhotoResponseDto?> AddPhotoAsync(int placeId, CreatePhotoDto dto);
        Task<List<PhotoResponseDto>> GetPlacePhotosAsync(int placeId);
        Task<bool> SetMainPhotoAsync(int placeId, int photoId);
        Task<bool> DeletePhotoAsync(int placeId, int photoId);
    }
}
