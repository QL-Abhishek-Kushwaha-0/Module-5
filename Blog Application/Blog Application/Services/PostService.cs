using Blog_Application.Data;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Helper;
using Blog_Application.Models.Entities;
using Blog_Application.Resources;
using Blog_Application.Utils;
using Microsoft.EntityFrameworkCore;

namespace Blog_Application.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly S3Service _s3Service;
        public PostService(ApplicationDbContext context, S3Service s3Service)
        {
            _context = context;
            _s3Service = s3Service;
        }

        public async Task<List<PostResponseDto>> GetCategoryPosts(int categoryId)
        {
            var posts = await _context.Posts
                .Where(c => categoryId == c.CategoryId)
                .Include(c => c.Category)
                .Include(c => c.Author)
                .Where(c => c.IsPublished == true)
                .Select(c => new PostResponseDto
                {
                    Title = c.Title,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl ?? "",
                    Category = c.Category != null ? c.Category.Name : "Other",
                    Author = c.Author.Name
                })
                .ToListAsync();

            return posts;
        }

        public async Task<List<PostResponseDto>> GetAllPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Where(p => p.IsPublished == true)
                .Select(p => new PostResponseDto
                {
                    Title = p.Title,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl ?? "",
                    Category = p.Category != null ? p.Category.Name : "Other",
                    Author = p.Author.Name
                })
                .ToListAsync();
            return posts;
        }

        public async Task<List<PostResponseDto>> GetAuthorPosts(Guid authorId)
        {
            var user = await _context.Users.Include(a => a.Posts!).ThenInclude(c => c.Category).FirstOrDefaultAsync(a => a.Id == authorId);

            if (user == null) return new List<PostResponseDto>();

            var postList = user.Posts!.Select(p => new PostResponseDto
            {
                Title = p.Title,
                Description = p.Description,
                ImageUrl = p.ImageUrl ?? "",
                Category = p.Category != null ? p.Category.Name : "Other",
                Author = user.Name
            }).ToList();

            return postList;
        }

        public async Task<PostResponseDto> GetPostById(int postId)
        {
            var post = await _context.Posts.Include(p => p.Category).Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return null;

            var postRes = new PostResponseDto
            {
                Title = post.Title,
                Description = post.Description,
                ImageUrl = post.ImageUrl,
                Category = post.Category != null ? post.Category.Name : "Other",
                Author = post.Author.Name
            };

            return postRes;
        }

        public async Task<PostResponseDto> UpdatePost(PostDto postDto, int postId, Guid authorId)
        {
            var post = await _context.Posts.Include(p => p.Category).Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return null;
            if (post.AuthorId != authorId) return null;

            post.Title = postDto.Title;
            post.Description = postDto.Description;
            post.ImageUrl = postDto.ImageUrl ?? post.ImageUrl;

            await _context.SaveChangesAsync();

            var postRes = new PostResponseDto
            {
                Title = postDto.Title,
                Description = postDto.Description,
                ImageUrl = postDto.ImageUrl ?? post.ImageUrl,
                Category = post.Category != null ? post.Category.Name : "Other",
                Author = post.Author.Name
            };

            return postRes;
        }

        public async Task<string> UploadImage(int postId, IFormFile image, HttpRequest request)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return "NoPostFound";

            var fileName = await HelperFunctions.GetFileName(image, _s3Service);    // Will upload the image to images folder and return the name of the stored image

            if (fileName.Equals("InvalidImage")) return fileName;

            post.ImageUrl = fileName;

            await _context.SaveChangesAsync();

            return fileName;
        }

        public async Task<PostResponseDto> CreatePost(int categoryId, PostDto postDto, Guid authorId)
        {
            var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Title.ToLower() == postDto.Title.ToLower());

            if (existingPost != null) return null;

            var category = await _context.Categories
                    .Include(c => c.Author)
                    .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null) return null;

            var author = await _context.Users.FirstOrDefaultAsync(u => u.Id == authorId);

            var newPost = new Post
            {
                Title = postDto.Title,
                Description = postDto.Description,
                ImageUrl = postDto.ImageUrl ?? "",
                IsPublished = false,
                CategoryId = categoryId,
                AuthorId = authorId,
            };

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            return new PostResponseDto { Title = newPost.Title, Description = newPost.Title, ImageUrl = newPost.ImageUrl, Category = category.Name, Author = author.Name };
        }

        public async Task<string> PublishPost(int postId, Guid authorId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return "NoPostFound";

            if (post.AuthorId != authorId) return "UnAuthorized";

            if (post.IsPublished == true) return "AlreadyPublished";

            post.IsPublished = true;
            await _context.SaveChangesAsync();

            return "Success";
        }

        public async Task<string> UnpublishPost(int postId, Guid authorId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return "NoPostFound";

            if (post.AuthorId != authorId) return "UnAuthorized";

            if (post.IsPublished == false) return "NotPublishedYet";

            post.IsPublished = false;
            await _context.SaveChangesAsync();

            return "Success";
        }

        public async Task<string> DeletePost(int postId, Guid authorId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return "NoPostFound";

            if (post.AuthorId != authorId) return "InvalidAuthor";

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return "Success";
        }
    }
}
