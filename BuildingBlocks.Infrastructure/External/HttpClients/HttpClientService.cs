using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.External.HttpClients;

public class HttpClientService : IHttpClientService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly HttpClientConfiguration _configuration;
    private readonly ILogger<HttpClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private static readonly ActivitySource ActivitySource = new("BuildingBlocks.Infrastructure.HttpClient");

    public HttpClientService(
        HttpClient httpClient,
        IOptions<HttpClientConfiguration> configuration,
        ILogger<HttpClientService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        ConfigureHttpClient();
    }

    public async Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(uri);
        return await ExecuteWithRetryAsync(
            () => _httpClient.GetAsync(new Uri(uri, UriKind.RelativeOrAbsolute), cancellationToken),
            "GET",
            uri,
            cancellationToken);
    }

    public async Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        return await ExecuteWithRetryAsync(
            () => _httpClient.GetAsync(uri, cancellationToken),
            "GET",
            uri.ToString(),
            cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(uri);
        using var activity = ActivitySource.StartActivity("HttpClient.Get");
        activity?.SetTag("http.method", "GET");
        activity?.SetTag("http.uri", uri);

        var response = await GetAsync(new Uri(uri, UriKind.RelativeOrAbsolute), cancellationToken);
        return await DeserializeResponseAsync<T>(response, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(Uri uri, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        using var activity = ActivitySource.StartActivity("HttpClient.Get");
        activity?.SetTag("http.method", "GET");
        activity?.SetTag("http.uri", uri.ToString());

        var response = await GetAsync(uri, cancellationToken);
        return await DeserializeResponseAsync<T>(response, cancellationToken);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string uri, T data, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(uri);
        using var content = CreateJsonContent(data);
        return await ExecuteWithRetryAsync(
            () => _httpClient.PostAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, cancellationToken),
            "POST",
            uri,
            cancellationToken);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(Uri uri, T data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        using var content = CreateJsonContent(data);
        return await ExecuteWithRetryAsync(
            () => _httpClient.PostAsync(uri, content, cancellationToken),
            "POST",
            uri.ToString(),
            cancellationToken);
    }

    public async Task<TResponse?> PostAsync<T, TResponse>(string uri, T data, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(uri);
        using var activity = ActivitySource.StartActivity("HttpClient.Post");
        activity?.SetTag("http.method", "POST");
        activity?.SetTag("http.uri", uri);

        var response = await PostAsync(new Uri(uri, UriKind.RelativeOrAbsolute), data, cancellationToken);
        return await DeserializeResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<TResponse?> PostAsync<T, TResponse>(Uri uri, T data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        using var activity = ActivitySource.StartActivity("HttpClient.Post");
        activity?.SetTag("http.method", "POST");
        activity?.SetTag("http.uri", uri.ToString());

        var response = await PostAsync(uri, data, cancellationToken);
        return await DeserializeResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string uri, T data, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(uri);
        using var content = CreateJsonContent(data);
        return await ExecuteWithRetryAsync(
            () => _httpClient.PutAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, cancellationToken),
            "PUT",
            uri,
            cancellationToken);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(Uri uri, T data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        using var content = CreateJsonContent(data);
        return await ExecuteWithRetryAsync(
            () => _httpClient.PutAsync(uri, content, cancellationToken),
            "PUT",
            uri.ToString(),
            cancellationToken);
    }

    public async Task<TResponse?> PutAsync<T, TResponse>(string uri, T data, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(uri);
        using var activity = ActivitySource.StartActivity("HttpClient.Put");
        activity?.SetTag("http.method", "PUT");
        activity?.SetTag("http.uri", uri);

        var response = await PutAsync(new Uri(uri, UriKind.RelativeOrAbsolute), data, cancellationToken);
        return await DeserializeResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<TResponse?> PutAsync<T, TResponse>(Uri uri, T data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        using var activity = ActivitySource.StartActivity("HttpClient.Put");
        activity?.SetTag("http.method", "PUT");
        activity?.SetTag("http.uri", uri.ToString());

        var response = await PutAsync(uri, data, cancellationToken);
        return await DeserializeResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(uri);
        return await ExecuteWithRetryAsync(
            () => _httpClient.DeleteAsync(new Uri(uri, UriKind.RelativeOrAbsolute), cancellationToken),
            "DELETE",
            uri,
            cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        return await ExecuteWithRetryAsync(
            () => _httpClient.DeleteAsync(uri, cancellationToken),
            "DELETE",
            uri.ToString(),
            cancellationToken);
    }

    public async Task<HttpResponseMessage> PatchAsync<T>(string uri, T data, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(uri);
        using var content = CreateJsonContent(data);
        return await ExecuteWithRetryAsync(
            () => _httpClient.PatchAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, cancellationToken),
            "PATCH",
            uri,
            cancellationToken);
    }

    public async Task<HttpResponseMessage> PatchAsync<T>(Uri uri, T data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        using var content = CreateJsonContent(data);
        return await ExecuteWithRetryAsync(
            () => _httpClient.PatchAsync(uri, content, cancellationToken),
            "PATCH",
            uri.ToString(),
            cancellationToken);
    }

    public void SetAuthorizationHeader(string scheme, string parameter)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme, parameter);
        _logger.LogDebug("Authorization header set with scheme: {Scheme}", scheme);
    }

    public void AddDefaultHeader(string name, string value)
    {
        _httpClient.DefaultRequestHeaders.Add(name, value);
        _logger.LogDebug("Default header added: {Name} = {Value}", name, value);
    }

    private void ConfigureHttpClient()
    {
        if (!string.IsNullOrEmpty(_configuration.BaseAddress))
        {
            _httpClient.BaseAddress = new Uri(_configuration.BaseAddress);
        }

        _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.TimeoutSeconds);

        foreach (var header in _configuration.DefaultHeaders)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        _logger.LogInformation("HTTP Client configured with base address: {BaseAddress}, timeout: {Timeout}s",
            _configuration.BaseAddress, _configuration.TimeoutSeconds);
    }

    private async Task<HttpResponseMessage> ExecuteWithRetryAsync(
        Func<Task<HttpResponseMessage>> operation,
        string method,
        string uri,
        CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity($"HttpClient.{method}");
        activity?.SetTag("http.method", method);
        activity?.SetTag("http.uri", uri);

        if (!_configuration.Retry.Enabled)
        {
            return await operation();
        }

        var attempt = 0;
        var delay = _configuration.Retry.BaseDelayMilliseconds;

        while (attempt < _configuration.Retry.MaxAttempts)
        {
            attempt++;
            
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await operation();
                stopwatch.Stop();

                activity?.SetTag("http.status_code", (int)response.StatusCode);
                activity?.SetTag("http.response_time_ms", stopwatch.ElapsedMilliseconds);

                if (response.IsSuccessStatusCode || 
                    !_configuration.Retry.RetryStatusCodes.Contains((int)response.StatusCode))
                {
                    if (attempt > 1)
                    {
                        _logger.LogInformation("HTTP {Method} {Uri} succeeded on attempt {Attempt}", 
                            method, uri, attempt);
                    }
                    return response;
                }

                if (attempt == _configuration.Retry.MaxAttempts)
                {
                    _logger.LogWarning("HTTP {Method} {Uri} failed after {MaxAttempts} attempts. Status: {StatusCode}",
                        method, uri, _configuration.Retry.MaxAttempts, response.StatusCode);
                    return response;
                }

                _logger.LogWarning("HTTP {Method} {Uri} failed on attempt {Attempt} with status {StatusCode}. Retrying in {Delay}ms",
                    method, uri, attempt, response.StatusCode, delay);

                response.Dispose();
            }
            catch (Exception ex) when (attempt < _configuration.Retry.MaxAttempts)
            {
                _logger.LogWarning(ex, "HTTP {Method} {Uri} failed on attempt {Attempt}. Retrying in {Delay}ms",
                    method, uri, attempt, delay);
                
                activity?.SetTag("exception.type", ex.GetType().Name);
                activity?.SetTag("exception.message", ex.Message);
            }

            if (attempt < _configuration.Retry.MaxAttempts)
            {
                await Task.Delay(delay, cancellationToken);
                delay = Math.Min(
                    (int)(delay * _configuration.Retry.BackoffMultiplier),
                    _configuration.Retry.MaxDelayMilliseconds);
            }
        }

        throw new HttpRequestException($"HTTP {method} {uri} failed after {_configuration.Retry.MaxAttempts} attempts");
    }

    private StringContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrEmpty(content))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response content to {Type}", typeof(T).Name);
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _httpClient?.Dispose();
            ActivitySource?.Dispose();
        }
    }
}