using Blog_Application.Data;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Enums;
using Blog_Application.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog_Application.Services
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;

        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LikeResponse> LikePost(int postId, Guid userId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null || post.IsPublished == false)
                return LikeResponse.NotFound;

            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
                return LikeResponse.AlreadyLiked;

            var newLike = new Like
            {
                PostId = postId,
                UserId = userId
            };

            _context.Likes.Add(newLike);
            await _context.SaveChangesAsync();

            return LikeResponse.Success;
        }

        public async Task<LikeResponse> UnlikePost(int postId, Guid userId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null || post.IsPublished == false) return LikeResponse.NotFound;

            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike == null) return LikeResponse.NotYetLiked;

            _context.Likes.Remove(existingLike);
            await _context.SaveChangesAsync();

            return LikeResponse.Success;
        }

        public async Task<CommentResponseDto> Comment(int postId, Guid userId, CommentDto commentDto)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null || post.IsPublished == false) return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var comment = new Comment
            {
                Content = commentDto.Content,
                PostId = postId,
                UserId = userId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return new CommentResponseDto { Content = commentDto.Content, Post = post.Title, Username = user == null ? "Unknown" : user.Name };
        }

        public async Task<CommentResponse> DeleteComment(int postId, int commentId, Guid userId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if (post == null || comment == null) return CommentResponse.NotFound;

            if (comment.UserId != userId) return CommentResponse.Unauthorized;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return CommentResponse.Success;
        }

        public async Task<List<CommentResponseDto>> GetPostComments(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.Comments!)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return new List<CommentResponseDto>();

            var comments = post.Comments!
                .Select(c => new CommentResponseDto
                {
                    Content = c.Content,
                    Post = post.Title,
                    Username = c.User.Name
                }).ToList();

            return comments;
        }
    }
}
