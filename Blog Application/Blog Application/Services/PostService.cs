using Blog_Application.Data;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Helper;
using Blog_Application.Models.Entities;
using Blog_Application.Resources;
using Blog_Application.Utils;
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

            await _context.Posts.DeleteOneAsync(Builders<Post>.Filter.Eq(p => p.Id, postId));

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

        //public async Task<List<PostResponseDto>> GetCategoryPosts(int categoryId)
        //{
        //    var posts = await _context.Posts
        //        .Where(c => categoryId == c.CategoryId)
        //        .Include(c => c.Category)
        //        .Include(c => c.Author)
        //        .Where(c => c.IsPublished == true)
        //        .Select(c => new PostResponseDto
        //        {
        //            Title = c.Title,
        //            Description = c.Description,
        //            ImageUrl = c.ImageUrl ?? "",
        //            Category = c.Category != null ? c.Category.Name : "Other",
        //            Author = c.Author.Name
        //        })
        //        .ToListAsync();

        //    return posts;
        //}

        //public async Task<List<PostResponseDto>> GetAllPosts()
        //{
        //    var posts = await _context.Posts
        //        .Include(p => p.Author)
        //        .Include(p => p.Category)
        //        .Where(p => p.IsPublished == true)
        //        .Select(p => new PostResponseDto
        //        {
        //            Title = p.Title,
        //            Description = p.Description,
        //            ImageUrl = p.ImageUrl ?? "",
        //            Category = p.Category != null ? p.Category.Name : "Other",
        //            Author = p.Author.Name
        //        })
        //        .ToListAsync();
        //    return posts;
        //}

        //public async Task<List<PostResponseDto>> GetAuthorPosts(Guid authorId)
        //{
        //    var user = await _context.Users.Include(a => a.Posts!).ThenInclude(c => c.Category).FirstOrDefaultAsync(a => a.Id == authorId);

        //    if (user == null) return new List<PostResponseDto>();

        //    var postList = user.Posts!.Select(p => new PostResponseDto
        //    {
        //        Title = p.Title,
        //        Description = p.Description,
        //        ImageUrl = p.ImageUrl ?? "",
        //        Category = p.Category != null ? p.Category.Name : "Other",
        //        Author = user.Name
        //    }).ToList();

        //    return postList;
        //}


        //public async Task<string> PublishPost(int postId, Guid authorId)
        //{
        //    var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

        //    if (post == null) return "NoPostFound";

        //    if (post.AuthorId != authorId) return "UnAuthorized";

        //    if (post.IsPublished == true) return "AlreadyPublished";

        //    post.IsPublished = true;
        //    await _context.SaveChangesAsync();

        //    return "Success";
        //}

        //public async Task<string> UnpublishPost(int postId, Guid authorId)
        //{
        //    var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

        //    if (post == null) return "NoPostFound";

        //    if (post.AuthorId != authorId) return "UnAuthorized";

        //    if (post.IsPublished == false) return "NotPublishedYet";

        //    post.IsPublished = false;
        //    await _context.SaveChangesAsync();

        //    return "Success";
        //}


    }
}
