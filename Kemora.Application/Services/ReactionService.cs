using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Kemora.Application.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _reactionRepo;
        private readonly IPostRepository _postRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public ReactionService(IReactionRepository reactionRepo, IPostRepository postRepo, ICommentRepository commentRepo, IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _reactionRepo = reactionRepo;
            _postRepo = postRepo;
            _commentRepo = commentRepo;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<bool> ReactToPostAsync(string userId, int postId, ReactToPostDto dto)
        {
            if (!await _postRepo.ExistsAsync(postId)) return false;

            var existing = await _reactionRepo.GetPostReactionAsync(postId, userId);
            if (dto.Action == "add")
            {
                if (existing != null)
                {
                    existing.ReactionType = dto.ReactionType;
                    existing.ReactedAt = DateTime.UtcNow;
                }
                else
                {
                    await _reactionRepo.AddPostReactionAsync(new PostReaction
                    {
                        PostID = postId, UserID = userId, ReactionType = dto.ReactionType, ReactedAt = DateTime.UtcNow
                    });
                }
            }
            else
            {
                if (existing != null) _reactionRepo.RemovePostReaction(existing);
            }

            await _unitOfWork.CommitAsync();

            // Notify post owner about the reaction
            if (dto.Action == "add")
            {
                var post = await _postRepo.GetByIdAsync(postId);
                if (post != null && post.UserID != userId)
                {
                    await _notificationService.CreateNotificationAsync(
                        post.UserID, "New Reaction", $"Someone reacted to your post with {dto.ReactionType}");
                }
            }

            return true;
        }

        public async Task<bool> ReactToCommentAsync(string userId, int commentId, ReactToCommentDto dto)
        {
            if (!await _commentRepo.ExistsAsync(commentId)) return false;

            var existing = await _reactionRepo.GetCommentReactionAsync(commentId, userId);
            if (dto.Action == "add")
            {
                if (existing != null)
                {
                    existing.ReactionType = dto.ReactionType;
                    existing.ReactedAt = DateTime.UtcNow;
                }
                else
                {
                    await _reactionRepo.AddCommentReactionAsync(new CommentReaction
                    {
                        CommentID = commentId, UserID = userId, ReactionType = dto.ReactionType, ReactedAt = DateTime.UtcNow
                    });
                }
            }
            else
            {
                if (existing != null) _reactionRepo.RemoveCommentReaction(existing);
            }

            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
