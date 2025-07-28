namespace BuildingBlocks.Infrastructure.Monitoring.Health;

public class HealthCheckConfiguration
{
    public DatabaseHealthCheckOptions Database { get; set; } = new();
    public MemoryHealthCheckOptions Memory { get; set; } = new();
    public HttpHealthCheckOptions Http { get; set; } = new();
    public RedisHealthCheckOptions Redis { get; set; } = new();
    public SmtpHealthCheckOptions Smtp { get; set; } = new();
    public List<CustomHealthCheckOptions> Custom { get; set; } = new();
}

public class DatabaseHealthCheckOptions
{
    public bool Enabled { get; set; } = true;
    public string Name { get; set; } = "Database";
    public int TimeoutSeconds { get; set; } = 30;
    public string? ConnectionString { get; set; }
    public List<string> Tags { get; set; } = new() { "database", "sql" };
}

public class MemoryHealthCheckOptions
{
    public bool Enabled { get; set; } = true;
    public string Name { get; set; } = "Memory";
    public long ThresholdBytes { get; set; } = 1024 * 1024 * 1024; // 1GB
    public List<string> Tags { get; set; } = new() { "memory", "system" };
}

public class HttpHealthCheckOptions
{
    public bool Enabled { get; set; } = false;
    public string Name { get; set; } = "Http";
    public string Url { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 10;
    public List<int> ExpectedStatusCodes { get; set; } = new() { 200 };
    public Dictionary<string, string> Headers { get; set; } = new();
    public List<string> Tags { get; set; } = new() { "http", "external" };
}

public class RedisHealthCheckOptions
{
    public bool Enabled { get; set; } = false;
    public string Name { get; set; } = "Redis";
    public string ConnectionString { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 10;
    public List<string> Tags { get; set; } = new() { "redis", "cache" };
}

public class SmtpHealthCheckOptions
{
    public bool Enabled { get; set; } = false;
    public string Name { get; set; } = "Smtp";
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public int TimeoutSeconds { get; set; } = 10;
    public List<string> Tags { get; set; } = new() { "smtp", "email" };
}

public class CustomHealthCheckOptions
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public Dictionary<string, string> Parameters { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}