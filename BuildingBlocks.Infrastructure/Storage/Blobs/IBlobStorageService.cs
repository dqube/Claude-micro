namespace BuildingBlocks.Infrastructure.Storage.Blobs;

public interface IBlobStorageService
{
    Task<string> UploadAsync(string containerName, string blobName, Stream stream, CancellationToken cancellationToken = default);
    Task<string> UploadAsync(string containerName, string blobName, byte[] data, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadBytesAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    Task DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    Task<IEnumerable<BlobInfo>> ListAsync(string containerName, CancellationToken cancellationToken = default);
    Task<string> GetDownloadUrlAsync(string containerName, string blobName, TimeSpan expiry, CancellationToken cancellationToken = default);
}

public class BlobInfo
{
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string ContentType { get; set; } = string.Empty;
}