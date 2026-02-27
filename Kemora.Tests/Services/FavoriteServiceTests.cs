using AutoMapper;
using FluentAssertions;
using Kemora.Application.DTOs;
using Kemora.Application.Services;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Kemora.Tests.Services
{
    public class FavoriteServiceTests
    {
        private readonly Mock<IFavoriteRepository> _mockFavRepo;
        private readonly Mock<IPlaceRepository> _mockPlaceRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly FavoriteService _service;

        public FavoriteServiceTests()
        {
            _mockFavRepo = new Mock<IFavoriteRepository>();
            _mockPlaceRepo = new Mock<IPlaceRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _service = new FavoriteService(
                _mockFavRepo.Object,
                _mockPlaceRepo.Object,
                _mockMapper.Object,
                _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AddFavoriteAsync_WhenPlaceDoesNotExist_ReturnsFalse()
        {
            // Arrange
            _mockPlaceRepo.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

            // Act
            var result = await _service.AddFavoriteAsync("user1", 1);

            // Assert
            result.Should().BeFalse();
            _mockFavRepo.Verify(r => r.AddAsync(It.IsAny<UserFavorite>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task AddFavoriteAsync_WhenAlreadyFavorited_ReturnsFalse()
        {
            // Arrange
            _mockPlaceRepo.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockFavRepo.Setup(r => r.IsFavoritedAsync("user1", 1)).ReturnsAsync(true);

            // Act
            var result = await _service.AddFavoriteAsync("user1", 1);

            // Assert
            result.Should().BeFalse();
            _mockFavRepo.Verify(r => r.AddAsync(It.IsAny<UserFavorite>()), Times.Never);
        }

        [Fact]
        public async Task AddFavoriteAsync_WhenValid_ReturnsTrueAndCommits()
        {
            // Arrange
            _mockPlaceRepo.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockFavRepo.Setup(r => r.IsFavoritedAsync("user1", 1)).ReturnsAsync(false);

            // Act
            var result = await _service.AddFavoriteAsync("user1", 1);

            // Assert
            result.Should().BeTrue();
            _mockFavRepo.Verify(r => r.AddAsync(It.Is<UserFavorite>(f => f.UserID == "user1" && f.PlaceID == 1)), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
