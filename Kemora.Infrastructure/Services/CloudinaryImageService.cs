using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Kemora.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Services
{
    public class CloudinaryImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageService(IConfiguration config)
        {
            var acc = new Account(
                config["Cloudinary:CloudName"] ?? "dddhzbrqy",
                config["Cloudinary:ApiKey"] ?? "269465313852975",
                config["Cloudinary:ApiSecret"] ?? "J9XF2lHIpe4IYwY6HjGt_5kedJ8"
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string?> UploadImageAsync(Stream fileStream, string fileName)
        {
            if (fileStream.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Transformation = new Transformation().Height(1000).Width(1000).Crop("limit")
                };
                
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl?.ToString();
            }

            return null;
        }
    }
}
