using AutoMapper;
using FluentAssertions;
using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Application.Services;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Kemora.Tests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> _mockCommentRepo;
        private readonly Mock<IPostRepository> _mockPostRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly CommentService _service;

        public CommentServiceTests()
        {
            _mockCommentRepo = new Mock<ICommentRepository>();
            _mockPostRepo = new Mock<IPostRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockNotificationService = new Mock<INotificationService>();

            _service = new CommentService(
                _mockCommentRepo.Object,
                _mockPostRepo.Object,
                _mockUserRepo.Object,
                _mockMapper.Object,
                _mockUnitOfWork.Object,
                _mockNotificationService.Object);
        }

        [Fact]
        public async Task CreateCommentAsync_WhenPostDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockPostRepo.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _service.CreateCommentAsync(999, "user1", new CreateCommentDto { Content = "test" });

            // Assert
            result.Should().BeNull();
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateCommentAsync_WhenValid_ReturnsDto_AndNotifiesPostOwner()
        {
            // Arrange
            _mockPostRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _mockUserRepo.Setup(r => r.GetByIdAsync("user2")).ReturnsAsync(new ApplicationUser { Id = "user2", FullName = "Commenter" });
            _mockMapper.Setup(m => m.Map<CommentResponseDto>(It.IsAny<Comment>())).Returns(new CommentResponseDto { Content = "test" });
            _mockPostRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostID = 1, UserID = "user1" });

            // Act
            var result = await _service.CreateCommentAsync(1, "user2", new CreateCommentDto { Content = "test" });

            // Assert
            result.Should().NotBeNull();
            result!.AuthorName.Should().Be("Commenter");
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockNotificationService.Verify(n => n.CreateNotificationAsync("user1", "New Comment", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateCommentAsync_WhenCommentingOwnPost_DoesNotNotify()
        {
            // Arrange
            _mockPostRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _mockUserRepo.Setup(r => r.GetByIdAsync("user1")).ReturnsAsync(new ApplicationUser { Id = "user1", FullName = "Owner" });
            _mockMapper.Setup(m => m.Map<CommentResponseDto>(It.IsAny<Comment>())).Returns(new CommentResponseDto { Content = "test" });
            _mockPostRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostID = 1, UserID = "user1" });

            // Act
            var result = await _service.CreateCommentAsync(1, "user1", new CreateCommentDto { Content = "test" });

            // Assert
            result.Should().NotBeNull();
            _mockNotificationService.Verify(n => n.CreateNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
