using System.Net;
using Amazon.S3;
using ErrorOr;
using EShopApp.Application.Common.Interfaces.Services;

namespace EShopApp.Infrastructure.ImageStorage;

public class S3ImageStorageService : IImageStorageService
{
    private const string _bucketName = "masaftic-shop";
    private const string _bucketPath = "product-images";
    
    private readonly IAmazonS3 _amazonS3Client;

    public S3ImageStorageService(IAmazonS3 amazonS3Client)
    {
        _amazonS3Client = amazonS3Client;
    }

    public async Task<ErrorOr<string>> SaveAsync(string fileName, string contentType, byte[] binaryContent, CancellationToken cancellationToken)
    {
        // s3://masaftic-shop/product-images/

        using var stream = new MemoryStream(binaryContent);
        var request = new Amazon.S3.Model.PutObjectRequest
        {
            BucketName = _bucketName,
            Key = $"{_bucketPath}/{fileName}",
            InputStream = stream,
            ContentType = contentType
        };

        var response = await _amazonS3Client.PutObjectAsync(request, cancellationToken);
        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            return $"{_bucketPath}/{fileName}";
        }

        return Error.Failure("S3.UploadImage.Failed", $"Failed to upload file to S3, status code: {response.HttpStatusCode}");
    }

    public async Task<Stream> GetAsync(string key)
    {
        var request = new Amazon.S3.Model.GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        var response = await _amazonS3Client.GetObjectAsync(request);
        if (response.HttpStatusCode == HttpStatusCode.NotFound)
        {
            // TODO: handle not found
            throw new FileNotFoundException($"File not found: {key}");
        }

        return response.ResponseStream;
    }

    public string GetPresignedUrl(string key, TimeSpan expiration)
    {
        var request = new Amazon.S3.Model.GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Expires = DateTime.UtcNow.Add(expiration)
        };

        return _amazonS3Client.GetPreSignedURL(request);
    }

    public async Task<List<string>> ListAsync()
    {
        var request = new Amazon.S3.Model.ListObjectsV2Request
        {
            BucketName = _bucketName,
            Prefix = $"{_bucketPath}/"
        };

        var response = await _amazonS3Client.ListObjectsV2Async(request);
        var fileNames = new List<string>();
        foreach (var entry in response.S3Objects)
        {
            fileNames.Add(entry.Key);
        }
        return fileNames;
    }

    public async Task<ErrorOr<bool>> DeleteAsync(string key)
    {
        var request = new Amazon.S3.Model.DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        var response = await _amazonS3Client.DeleteObjectAsync(request);
        if (response.HttpStatusCode == HttpStatusCode.NoContent)
        {
            return true;
        }

        return Error.Failure("S3.DeleteImage.Failed", $"Failed to delete file from S3, status code: {response.HttpStatusCode}");
    }
}
