using Blog_Application.Data;
using Blog_Application.DTO;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Enums;
using Blog_Application.Helper;
using Blog_Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Blog_Application.Services
{
    public class BlogService : IBlogService
    {
        private readonly MongoDbContext _context;

        public BlogService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<LikeResponse> LikePost(string postId, string userId)
        {
            var postExists = await _context.Posts.Find(p => p.Id == postId && p.IsPublished == true).AnyAsync();

            if (!postExists) return LikeResponse.NotFound;

            var existingLike = await _context.Likes.Find(l => l.PostId == postId && l.UserId == userId).AnyAsync();

            if (existingLike) return LikeResponse.AlreadyLiked;

            var newLike = new Like
            {
                PostId = postId,
                UserId = userId
            };

            await _context.Likes.InsertOneAsync(newLike);

            return LikeResponse.Success;
        }

        public async Task<LikeResponse> UnlikePost(string postId, string userId)
        {
            var postExists = await _context.Posts.Find(p => p.Id == postId && p.IsPublished == true).AnyAsync();

            if (!postExists) return LikeResponse.NotFound;

            var existingLike = await _context.Likes.Find(l => l.PostId == postId && l.UserId == userId).FirstOrDefaultAsync();

            if (existingLike == null) return LikeResponse.NotYetLiked;

            await _context.Likes.DeleteOneAsync(l => l.Id == existingLike.Id);

            return LikeResponse.Success;
        }

        public async Task<CommentResponseDto> Comment(string postId, string userId, CommentDto commentDto)
        {
            var post = await _context.Posts.Find(p => p.Id == postId).FirstOrDefaultAsync();

            if (post == null || post.IsPublished == false) return null;

            var user = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();

            var comment = new Comment
            {
                Content = commentDto.Content,
                PostId = postId,
                UserId = userId
            };

            await _context.Comments.InsertOneAsync(comment);

            return new CommentResponseDto { Content = commentDto.Content, Post = post.Title, Username = user == null ? "Unknown" : user.Name };
        }

        public async Task<CommentResponse> DeleteComment(string postId, string commentId, string userId)
        {
            var post = await _context.Posts.Find(p => p.Id == postId).FirstOrDefaultAsync();
            var comment = await _context.Comments.Find(c => c.Id == commentId).FirstOrDefaultAsync();

            if (post == null || comment == null) return CommentResponse.NotFound;

            if (comment.UserId != userId) return CommentResponse.Unauthorized;

            await _context.Comments
                .DeleteOneAsync(c => c.Id == commentId);

            return CommentResponse.Success;
        }

        public async Task<List<CommentResponseDto>> GetPostComments(string postId)
        {
            var post = await _context.Posts.Find(p => p.Id == postId).FirstOrDefaultAsync();

            if (post == null) return null;

            var comments = await _context.Comments
                .Aggregate()
                .Match(c => c.PostId == postId)
                .Lookup<Comment, User, LookupClasses.CommentWithUser>(
                    _context.Users,
                    c => c.UserId,
                    u => u.Id,
                    res => res.Author
                )
                .Project(c => new CommentResponseDto
                {
                    Content = c.Content,
                    Post = post.Title,
                    Username = c.Author!.FirstOrDefault()!.Name
                }).ToListAsync();

            return comments;
        }
    }
}
