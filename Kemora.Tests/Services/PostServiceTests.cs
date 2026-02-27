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
    public class PostServiceTests
    {
        private readonly Mock<IPostRepository> _mockPostRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly PostService _service;

        public PostServiceTests()
        {
            _mockPostRepo = new Mock<IPostRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _service = new PostService(
                _mockPostRepo.Object,
                _mockMapper.Object,
                _mockUserRepo.Object,
                _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateAsync_CreatesPostAndReturnsDto()
        {
            // Arrange
            var dto = new CreatePostDto { Content = "Test Post" };
            var user = new ApplicationUser { Id = "user1", FullName = "Test User" };

            _mockUserRepo.Setup(r => r.GetByIdAsync("user1")).ReturnsAsync(user);
            
            var expectedResponse = new PostListResponseDto { Content = "Test Post" };
            _mockMapper.Setup(m => m.Map<PostListResponseDto>(It.IsAny<Post>()))
                       .Returns(expectedResponse);

            // Act
            var result = await _service.CreateAsync("user1", dto);

            // Assert
            result.Should().NotBeNull();
            result.AuthorName.Should().Be("Test User");

            _mockPostRepo.Verify(r => r.AddAsync(It.Is<Post>(p => p.Content == "Test Post" && p.UserID == "user1")), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
