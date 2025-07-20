namespace BuildingBlocks.Infrastructure.Storage.Files;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null, CancellationToken cancellationToken = default);
    Task<string> UploadAsync(byte[] fileData, string fileName, string? contentType = null, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string fileId, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadBytesAsync(string fileId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string fileId, CancellationToken cancellationToken = default);
    Task DeleteAsync(string fileId, CancellationToken cancellationToken = default);
    Task<string> GetDownloadUrlAsync(string fileId, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task<FileMetadata> GetMetadataAsync(string fileId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FileMetadata>> ListFilesAsync(string? prefix = null, CancellationToken cancellationToken = default);
    Task<string> CopyAsync(string sourceFileId, string destinationFileName, CancellationToken cancellationToken = default);
    Task<string> MoveAsync(string sourceFileId, string destinationFileName, CancellationToken cancellationToken = default);
}

public class FileMetadata
{
    public string Id { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
    public string? ETag { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}