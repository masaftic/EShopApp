using ErrorOr;

namespace EShopApp.Application.Common.Interfaces.Services;


public interface IImageStorageService
{
    Task<ErrorOr<string>> SaveAsync(string fileName, string contentType, byte[] binaryContent, CancellationToken cancellationToken);
    Task<Stream> GetAsync(string key);
    Task<List<string>> ListAsync();
    public string GetPresignedUrl(string key, TimeSpan expiration);
    Task<ErrorOr<bool>> DeleteAsync(string key);
}