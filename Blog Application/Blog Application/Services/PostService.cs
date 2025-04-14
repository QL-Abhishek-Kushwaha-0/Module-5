using Blog_Application.Data;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Helper;
using Blog_Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Blog_Application.Services
{
    public class PostService : IPostService
    {
        private readonly MongoDbContext _context;
        public PostService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<PostResponseDto> CreatePost(string categoryId, PostDto postDto, string authorId)
        {
            var existingPost = await _context.Posts
                .Find(p => p.Title.ToLower() == postDto.Title.ToLower())
                .AnyAsync();

            if (existingPost)
                return null;

            var category = await _context.Categories.Find(c => c.Id == categoryId).FirstOrDefaultAsync();
            if (category == null) return null;

            var author = await _context.Users.Find(u => u.Id == authorId).FirstOrDefaultAsync();

            var newPost = new Post
            {
                Title = postDto.Title,
                Description = postDto.Description,
                ImageUrl = postDto.ImageUrl ?? "",
                IsPublished = false,
                CategoryId = categoryId,
                AuthorId = authorId,
            };

            await _context.Posts.InsertOneAsync(newPost);

            return new PostResponseDto { Title = newPost.Title, Description = newPost.Description, ImageUrl = newPost.ImageUrl, Category = category.Name, Author = author.Name };
        }

        public async Task<PostResponseDto> GetPostById(string postId)
        {
            var post = await _context.Posts.Find(p => p.Id == postId).FirstOrDefaultAsync();
            if (post == null || post.IsPublished == false) return null;

            var category = await _context.Categories.Find(c => c.Id == post.CategoryId).FirstOrDefaultAsync();
            var author = await _context.Users.Find(a => a.Id == post.AuthorId).FirstOrDefaultAsync();

            var postRes = new PostResponseDto
            {
                Title = post.Title,
                Description = post.Description,
                ImageUrl = post.ImageUrl,
                Category = category.Name,
                Author = author.Name
            };

            return postRes;
        }

        public async Task<PostResponseDto> UpdatePost(PostDto postDto, string postId, string authorId)
        {
            var post = await _context.Posts.Find(p => p.Id == postId).FirstOrDefaultAsync();

            if (post == null || post.AuthorId != authorId) return null;

            var category = await _context.Categories.Find(c => c.Id == post.CategoryId).FirstOrDefaultAsync();
            var author = await _context.Users.Find(a => a.Id == post.AuthorId).FirstOrDefaultAsync();

            var update = Builders<Post>.Update
                .Set(p => p.Title, postDto.Title)
                .Set(p => p.Description, postDto.Description)
                .Set(p => p.ImageUrl, postDto.ImageUrl ?? post.ImageUrl);

            await _context.Posts.UpdateOneAsync(p => p.Id == postId, update);

            return new PostResponseDto { Title = postDto.Title, Description = postDto.Description, ImageUrl = postDto.ImageUrl ?? post.ImageUrl, Category = category.Name, Author = author.Name };
        }

        public async Task<string> DeletePost(string postId, string authorId)
        {
            var post = await _context.Posts.Find(p => p.Id == postId).FirstOrDefaultAsync();

            if (post == null) return "NoPostFound";

            if (post.AuthorId != authorId) return "InvalidAuthor";

            await _context.Posts.DeleteOneAsync(p => p.Id == postId);

            await _context.Likes.DeleteManyAsync(l => l.PostId == postId);          // Deletes all related likes from Likes
            await _context.Comments.DeleteManyAsync(c => c.PostId == postId);    // Deletes all related comments from Comments

            return "Success";
        }

        public async Task<string> UploadImage(string postId, IFormFile image, HttpRequest request)
        {
            var post = await _context.Posts.Find(p => p.Id == postId).FirstOrDefaultAsync();

            if (post == null) return "NoPostFound";

            var fileName = await HelperFunctions.GetFileName(image);    // Will upload the image to images folder and return the name of the stored image

            if (fileName.Equals("InvalidImage")) return fileName;

            string imageUrl = $"{request.Scheme}://{request.Host}/images/{fileName}";

            var imageUpload = Builders<Post>.Update.Set(p => p.ImageUrl, imageUrl);

            await _context.Posts.UpdateOneAsync(
                Builders<Post>.Filter.Eq(p => p.Id, postId),
                imageUpload);

            return imageUrl;
        }

        public async Task<string> PublishPost(string postId, string authorId)
        {
            var post = await _context.Posts.Find(p => p.Id == postId).FirstOrDefaultAsync();

            if (post == null) return "NoPostFound";

            if (post.AuthorId != authorId) return "UnAuthorized";

            if (post.IsPublished == true) return "AlreadyPublished";

            await _context.Posts.UpdateOneAsync(
                Builders<Post>.Filter.Eq(p => p.Id, postId),
                Builders<Post>.Update.Set(p => p.IsPublished, true)
            );

            return "Success";
        }

        public async Task<string> UnpublishPost(string postId, string authorId)
        {
            var post = await _context.Posts.Find(p => p.Id == postId).FirstOrDefaultAsync();

            if (post == null) return "NoPostFound";

            if (post.AuthorId != authorId) return "UnAuthorized";

            if (post.IsPublished == false) return "NotPublishedYet";

            await _context.Posts.UpdateOneAsync(
                Builders<Post>.Filter.Eq(p => p.Id, postId),
                Builders<Post>.Update.Set(p => p.IsPublished, false)
            );

            return "Success";
        }

        public async Task<List<PostResponseDto>> GetAllPosts()
        {
            var posts = await _context.Posts
                .Aggregate()
                .Match(p => p.IsPublished == true)
                .Lookup<Post, Category, LookupClasses.PostWithCategory>(
                    _context.Categories,
                    p => p.CategoryId,
                    c => c.Id,
                    res => res.Category
                )
                .Lookup<LookupClasses.PostWithCategory, User, LookupClasses.PostWithAuthor>(
                    _context.Users,
                    wc => wc.AuthorId,
                    u => u.Id,
                    res => res.Author
                )
                .Project(p => new PostResponseDto
                {
                    Title = p.Title,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl ?? "",
                    Category = p.Category!.FirstOrDefault() != null ? p.Category!.FirstOrDefault()!.Name : "Others",
                    Author = p.Author!.FirstOrDefault()! != null ? p.Author!.FirstOrDefault()!.Name : "Unknown"
                })
                .ToListAsync();
            return posts;
        }


        public async Task<List<PostResponseDto>> GetAuthorPosts(string authorId)
        {
            var author = await _context.Users.Find(a => a.Id == authorId).FirstOrDefaultAsync();

            if (author == null) return null;

            var postList = await _context.Posts
                .Aggregate()
                .Match(p => p.AuthorId == authorId)
                .Lookup<Post, Category, LookupClasses.PostWithCategory>(
                    _context.Categories,
                    p => p.CategoryId,
                    c => c.Id,
                    res => res.Category
                )
                .Project(p => new PostResponseDto
                    {
                        Title = p.Title,
                        Description = p.Description,
                        ImageUrl = p.ImageUrl ?? "",
                        Category = p.Category != null ? p.Category.FirstOrDefault().Name : "Other",
                        Author = author.Name
                    }
                )
                .ToListAsync();

            return postList;
        }

        public async Task<List<PostResponseDto>> GetCategoryPosts(string categoryId)
        {
            var category = await _context.Categories.Find(c => c.Id == categoryId).FirstOrDefaultAsync();
            if (category == null) return null;

            var posts = await _context.Posts
                .Aggregate()
                .Match(p => p.CategoryId == categoryId && p.IsPublished == true)
                .Lookup<Post, User, LookupClasses.PostWithPostOwner>(
                    _context.Users, 
                    p => p.AuthorId,
                    u => u.Id,
                    res => res.Owner    
                )
                .Project(p => new PostResponseDto
                {
                    Title = p.Title,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl ?? "",
                    Category = category.Name,
                    Author = p.Owner!.FirstOrDefault()!.Name
                })
                .ToListAsync();

            return posts;
        }
    }
}
