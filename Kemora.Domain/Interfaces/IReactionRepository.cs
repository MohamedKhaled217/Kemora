using Kemora.Domain.Entities;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface IReactionRepository
    {
        Task<PostReaction?> GetPostReactionAsync(int postId, string userId);
        Task<CommentReaction?> GetCommentReactionAsync(int commentId, string userId);
        Task AddPostReactionAsync(PostReaction reaction);
        Task AddCommentReactionAsync(CommentReaction reaction);
        void RemovePostReaction(PostReaction reaction);
        void RemoveCommentReaction(CommentReaction reaction);

    }
}
