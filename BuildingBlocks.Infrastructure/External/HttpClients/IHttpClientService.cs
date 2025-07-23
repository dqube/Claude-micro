namespace BuildingBlocks.Infrastructure.External.HttpClients;

public interface IHttpClientService
{
    Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(string uri, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(Uri uri, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostAsync<T>(string uri, T data, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostAsync<T>(Uri uri, T data, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<T, TResponse>(string uri, T data, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<T, TResponse>(Uri uri, T data, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PutAsync<T>(string uri, T data, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PutAsync<T>(Uri uri, T data, CancellationToken cancellationToken = default);
    Task<TResponse?> PutAsync<T, TResponse>(string uri, T data, CancellationToken cancellationToken = default);
    Task<TResponse?> PutAsync<T, TResponse>(Uri uri, T data, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PatchAsync<T>(string uri, T data, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PatchAsync<T>(Uri uri, T data, CancellationToken cancellationToken = default);
    void SetAuthorizationHeader(string scheme, string parameter);
    void AddDefaultHeader(string name, string value);
}