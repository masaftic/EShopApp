using Amazon.Runtime;

namespace EShopApp.Infrastructure.ImageStorage;

public class AmazonS3HttpClientFactory : HttpClientFactory
{
    private readonly HttpClientHandler _handler;

    public AmazonS3HttpClientFactory(HttpClientHandler handler)
    {
        _handler = handler;
    }

    public override HttpClient CreateHttpClient(IClientConfig clientConfig)
    {
        return new HttpClient(_handler);
    }
}