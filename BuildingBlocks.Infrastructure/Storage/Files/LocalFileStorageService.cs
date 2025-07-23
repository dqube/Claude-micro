using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Storage.Files;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    private static readonly Action<ILogger, string, Exception?> LogFileUploaded =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1, "FileUploaded"), "File uploaded with ID: {FileId}");

    private static readonly Action<ILogger, string, Exception?> LogFileDeleted =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(2, "FileDeleted"), "File deleted with ID: {FileId}");

    private static readonly Action<ILogger, string, Exception?> LogMetadataReadError =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(3, "MetadataReadError"), "Failed to read metadata from file: {File}");

    public LocalFileStorageService(ILogger<LocalFileStorageService> logger, string basePath = "Storage/Files")
    {
        _logger = logger;
        _basePath = Path.GetFullPath(basePath);
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentNullException.ThrowIfNull(fileName);
        
        var fileId = Guid.NewGuid().ToString();
        var filePath = Path.Combine(_basePath, fileId);
        
        using var fileSystemStream = File.Create(filePath);
        await fileStream.CopyToAsync(fileSystemStream, cancellationToken);
        
        // Store metadata
        await SaveMetadataAsync(fileId, fileName, contentType, fileSystemStream.Length);
        
        LogFileUploaded(_logger, fileId, null);
        return fileId;
    }

    public async Task<string> UploadAsync(byte[] fileData, string fileName, string? contentType = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileData);
        ArgumentNullException.ThrowIfNull(fileName);
        
        using var stream = new MemoryStream(fileData);
        return await UploadAsync(stream, fileName, contentType, cancellationToken);
    }

    public async Task<Stream> DownloadAsync(string fileId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileId);
        
        var filePath = Path.Combine(_basePath, fileId);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File with ID {fileId} not found");
        }

        return await Task.FromResult(File.OpenRead(filePath));
    }

    public async Task<byte[]> DownloadBytesAsync(string fileId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileId);
        
        var filePath = Path.Combine(_basePath, fileId);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File with ID {fileId} not found");
        }

        return await File.ReadAllBytesAsync(filePath, cancellationToken);
    }

    public Task<bool> ExistsAsync(string fileId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileId);
        
        var filePath = Path.Combine(_basePath, fileId);
        return Task.FromResult(File.Exists(filePath));
    }

    public Task DeleteAsync(string fileId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileId);
        
        var filePath = Path.Combine(_basePath, fileId);
        var metadataPath = GetMetadataPath(fileId);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        if (File.Exists(metadataPath))
        {
            File.Delete(metadataPath);
        }
        
        LogFileDeleted(_logger, fileId, null);
        return Task.CompletedTask;
    }

    public Task<string> GetDownloadUrlAsync(string fileId, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileId);
        
        // For local storage, return a simple file path or URL
        // In a real implementation, this might generate a temporary URL
        var url = $"/files/{fileId}";
        return Task.FromResult(url);
    }

    public async Task<FileMetadata> GetMetadataAsync(string fileId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileId);
        
        var metadataPath = GetMetadataPath(fileId);
        if (!File.Exists(metadataPath))
        {
            throw new FileNotFoundException($"Metadata for file {fileId} not found");
        }

        var metadataJson = await File.ReadAllTextAsync(metadataPath, cancellationToken);
        var metadata = System.Text.Json.JsonSerializer.Deserialize<FileMetadata>(metadataJson);
        return metadata ?? throw new InvalidOperationException("Failed to deserialize file metadata");
    }

    public async Task<IEnumerable<FileMetadata>> ListFilesAsync(string? prefix = null, CancellationToken cancellationToken = default)
    {
        var files = Directory.GetFiles(_basePath, "*.json");
        var metadataList = new List<FileMetadata>();

        foreach (var file in files)
        {
            try
            {
                var metadataJson = await File.ReadAllTextAsync(file, cancellationToken);
                var metadata = System.Text.Json.JsonSerializer.Deserialize<FileMetadata>(metadataJson);
                if (metadata != null && (prefix == null || metadata.FileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                {
                    metadataList.Add(metadata);
                }
            }
            catch (FileNotFoundException ex)
            {
                LogMetadataReadError(_logger, file, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                LogMetadataReadError(_logger, file, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                LogMetadataReadError(_logger, file, ex);
            }
            catch (JsonException ex)
            {
                LogMetadataReadError(_logger, file, ex);
            }
            catch (IOException ex)
            {
                LogMetadataReadError(_logger, file, ex);
            }
        }

        return metadataList;
    }

    public async Task<string> CopyAsync(string sourceFileId, string destinationFileName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceFileId);
        ArgumentNullException.ThrowIfNull(destinationFileName);
        
        var sourcePath = Path.Combine(_basePath, sourceFileId);
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException($"Source file {sourceFileId} not found");
        }

        var destinationId = Guid.NewGuid().ToString();
        var destinationPath = Path.Combine(_basePath, destinationId);
        
        File.Copy(sourcePath, destinationPath);
        
        // Copy and update metadata
        var sourceMetadata = await GetMetadataAsync(sourceFileId, cancellationToken);
        sourceMetadata.Id = destinationId;
        sourceMetadata.FileName = destinationFileName;
        await SaveMetadataAsync(destinationId, destinationFileName, sourceMetadata.ContentType, sourceMetadata.Size);
        
        return destinationId;
    }

    public async Task<string> MoveAsync(string sourceFileId, string destinationFileName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceFileId);
        ArgumentNullException.ThrowIfNull(destinationFileName);
        
        var newFileId = await CopyAsync(sourceFileId, destinationFileName, cancellationToken);
        await DeleteAsync(sourceFileId, cancellationToken);
        return newFileId;
    }

    private async Task SaveMetadataAsync(string fileId, string fileName, string? contentType, long size)
    {
        var metadata = new FileMetadata
        {
            Id = fileId,
            FileName = fileName,
            ContentType = contentType,
            Size = size,
            CreatedAt = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        var metadataJson = System.Text.Json.JsonSerializer.Serialize(metadata);
        var metadataPath = GetMetadataPath(fileId);
        await File.WriteAllTextAsync(metadataPath, metadataJson);
    }

    private string GetMetadataPath(string fileId)
    {
        return Path.Combine(_basePath, $"{fileId}.json");
    }
}