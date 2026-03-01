using AutoMapper;
using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;

        private readonly IUnitOfWork _unitOfWork;

        public PostService(IPostRepository postRepo, IMapper mapper, IUserRepository userRepo, IUnitOfWork unitOfWork)
        {
            _postRepo = postRepo;
            _mapper = mapper;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<PostListResponseDto> CreateAsync(string userId, CreatePostDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            var post = new Post
            {
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow,
                UserID = userId,
                Media = dto.Media?.ConvertAll(m => new PostMedia { MediaURL = m.MediaURL, MediaType = m.MediaType })
            };

            await _postRepo.AddAsync(post);
            await _unitOfWork.CommitAsync();

            var response = _mapper.Map<PostListResponseDto>(post);
            response.AuthorName = user?.FullName ?? "Unknown";
            return response;
        }

        public async Task<PagedResult<PostListResponseDto>> GetPostsAsync(int page, int pageSize)
        {
            var posts = await _postRepo.GetPagedAsync(null, q => q.OrderByDescending(p => p.CreatedAt), page, pageSize, p => p.User, p => p.Media, p => p.Reactions, p => p.Comments);
            var count = await _postRepo.CountAsync();
            return new PagedResult<PostListResponseDto>
            {
                Items = _mapper.Map<List<PostListResponseDto>>(posts),
                TotalCount = count,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<PostDetailResponseDto?> GetPostAsync(int id)
        {
            var post = await _postRepo.GetByIdWithDetailsAsync(id);
            if (post == null) return null;
            return _mapper.Map<PostDetailResponseDto>(post);
        }

        public async Task<PagedResult<PostListResponseDto>> GetMyPostsAsync(string userId, int page, int pageSize)
        {
            var posts = await _postRepo.GetPagedAsync(p => p.UserID == userId, q => q.OrderByDescending(p => p.CreatedAt), page, pageSize, p => p.User, p => p.Media, p => p.Reactions, p => p.Comments);
            var count = await _postRepo.CountAsync(p => p.UserID == userId);
            return new PagedResult<PostListResponseDto>
            {
                Items = _mapper.Map<List<PostListResponseDto>>(posts),
                TotalCount = count,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> UpdatePostAsync(int id, string userId, UpdatePostDto dto)
        {
            var post = await _postRepo.GetByIdAsync(id);
            if (post == null || post.UserID != userId) return false;

            post.Content = dto.Content;
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeletePostAsync(int id, string userId)
        {
            var post = await _postRepo.GetByIdWithDetailsAsync(id);
            if (post == null || post.UserID != userId) return false;

            _postRepo.Remove(post);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
