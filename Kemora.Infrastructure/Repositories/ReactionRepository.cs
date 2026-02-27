using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly ApplicationDbContext _ctx;

        public ReactionRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<PostReaction?> GetPostReactionAsync(int postId, string userId)
        {
            return await _ctx.PostReactions
                .FirstOrDefaultAsync(r => r.PostID == postId && r.UserID == userId);
        }

        public async Task<CommentReaction?> GetCommentReactionAsync(int commentId, string userId)
        {
            return await _ctx.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentID == commentId && r.UserID == userId);
        }

        public async Task AddPostReactionAsync(PostReaction reaction) => await _ctx.PostReactions.AddAsync(reaction);
        public async Task AddCommentReactionAsync(CommentReaction reaction) => await _ctx.CommentReactions.AddAsync(reaction);

        public void RemovePostReaction(PostReaction reaction) => _ctx.PostReactions.Remove(reaction);
        public void RemoveCommentReaction(CommentReaction reaction) => _ctx.CommentReactions.Remove(reaction);


    }
}
