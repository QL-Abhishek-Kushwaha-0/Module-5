using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

public class S3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;

    public S3Service(IConfiguration configuration)
    {
        _configuration = configuration;

        var region = _configuration["AWS:Region"];
        // Initialize AWS S3 Client
        _s3Client = new AmazonS3Client(
            _configuration["AWS:AccessKey"],
            _configuration["AWS:SecretKey"],
            RegionEndpoint.GetBySystemName(region)
        );

        
    }

    public async Task<string> UploadFileAsync(string filePath)
    {
        var bucketName = _configuration["AWS:BucketName"];
        var keyName = Path.GetFileName(filePath);               // Extracts only the file name excluding the directories invloved

        try
        {
            var transferUtility = new TransferUtility(_s3Client);   // TransferUtility is helper class of AWS sdk that simplifies the process of uploading and downloading files from aws

            // Upload the file to S3
            await transferUtility.UploadAsync(filePath, bucketName);

            var cloudFrontUrl = "https://d1v7cem5hkknyx.cloudfront.net";

            var fileUrl = $"{cloudFrontUrl}/{keyName}";

            return fileUrl;
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex);
            throw new InvalidOperationException("Error uploading file to S3", ex);
        }
    }

    public async Task<string> GeneratePreSignedUrl(string imagePath, int expiryMinutes = 15)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _configuration["AWS:BucketName"],
            Key = imagePath,
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Verb = HttpVerb.PUT
        };

        string preSignedUrl = await _s3Client.GetPreSignedURLAsync(request);

        return preSignedUrl;
    }
}
