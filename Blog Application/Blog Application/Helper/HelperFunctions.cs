using Amazon.S3;
using Blog_Application.Middlewares;

namespace Blog_Application.Helper
{
    public class HelperFunctions
    {
        public static Guid GetGuid(string guid)
        {
            if (guid.Length == 0) return Guid.Empty;
            return Guid.TryParse(guid, out Guid resId) ? resId : Guid.Empty;
        }

        public static async Task<string> GetFileName(IFormFile image, S3Service _s3Service) // Removed static modifier
        {
            var fileExtension = Path.GetExtension(image.FileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            if (!allowedExtensions.Contains(fileExtension.ToLower()))
                return "InvalidImage";

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var origName = Path.GetFileNameWithoutExtension(image.FileName);

            var fileName = $"{origName}{Guid.NewGuid()}{fileExtension}"; // Name of stored image (original Name + Guid + OriginalExtension)

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var fileUrl = await _s3Service.UploadFileAsync(filePath); // Uploading the file to S3

            return fileUrl;
        }
    }
}
