using BuildingBlocks.Domain.StronglyTypedIds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Data.Converters.Examples;

/// <summary>
/// Examples showing different ways to configure strongly typed ID conversions in Entity Framework Core
/// </summary>
public static class StronglyTypedIdEfCoreExample
{
    /// <summary>
    /// Example strongly typed IDs for demonstration
    /// </summary>
    public class ProductId : GuidId
    {
        public ProductId(Guid value) : base(value) { }
        public ProductId() : base() { }
        
        public static ProductId From(Guid value) => new(value);
    }

    public class CategoryId : StronglyTypedId<int>
    {
        public CategoryId(int value) : base(value) { }
        
        public static CategoryId From(int value) => new(value);
    }

    public class SkuCode : StronglyTypedId<string>
    {
        public SkuCode(string value) : base(value) { }
        
        public static SkuCode From(string value) => new(value);
    }

    /// <summary>
    /// Example entity using multiple strongly typed IDs
    /// </summary>
    public class Product
    {
        public ProductId Id { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public CategoryId CategoryId { get; set; } = null!;
        public SkuCode Sku { get; set; } = null!;
        public decimal Price { get; set; }
    }

    public class Category
    {
        public CategoryId Id { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new();
    }

    /// <summary>
    /// Example DbContext using automatic strongly typed ID conversion
    /// </summary>
    public class ExampleDbContext : DbContext
    {
        public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Method 1: Use the automatic value converter selector (recommended)
            optionsBuilder.UseStronglyTypedIdConverters();
            
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Method 2: Use the automatic configuration extension (recommended)
            modelBuilder.ConfigureStronglyTypedIds();

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne<Category>()
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        }
    }

    /// <summary>
    /// Example showing manual configuration (for fine-grained control)
    /// </summary>
    public class ManualConfigurationExampleDbContext : DbContext
    {
        public ManualConfigurationExampleDbContext(DbContextOptions<ManualConfigurationExampleDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Method 3: Manual configuration for each strongly typed ID property
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                
                // Manual conversion configuration - you can specify column names, types, etc.
                entity.Property(p => p.Id)
                    .HasStronglyTypedIdConversion<ProductId, Guid>()
                    .HasColumnName("product_id")
                    .IsRequired();

                entity.Property(p => p.CategoryId)
                    .HasStronglyTypedIdConversion<CategoryId, int>()
                    .HasColumnName("category_id")
                    .IsRequired();

                entity.Property(p => p.Sku)
                    .HasStronglyTypedIdConversion<SkuCode, string>()
                    .HasColumnName("sku_code")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(p => p.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(p => p.Price)
                    .HasPrecision(18, 2);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                
                entity.Property(c => c.Id)
                    .HasStronglyTypedIdConversion<CategoryId, int>()
                    .HasColumnName("category_id")
                    .IsRequired();

                entity.Property(c => c.Name)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne<Category>()
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        }
    }

    /// <summary>
    /// Example entity configuration class showing strongly typed ID configuration
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            // Using the extension method for cleaner configuration
            builder.Property(p => p.Id)
                .HasStronglyTypedIdConversion<ProductId, Guid>();

            builder.Property(p => p.CategoryId)
                .HasStronglyTypedIdConversion<CategoryId, int>();

            builder.Property(p => p.Sku)
                .HasStronglyTypedIdConversion<SkuCode, string>()
                .HasMaxLength(50);

            builder.Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Price)
                .HasPrecision(18, 2);

            // Indexes
            builder.HasIndex(p => p.Sku)
                .IsUnique();

            builder.HasIndex(p => p.CategoryId);
        }
    }

    /// <summary>
    /// Example showing how to configure DbContext in dependency injection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExampleDbContext(
            this IServiceCollection services, 
            string connectionString)
        {
            services.AddDbContext<ExampleDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                
                // The strongly typed ID converters will be automatically applied
                // due to the UseStronglyTypedIdConverters() call in OnConfiguring
            });

            return services;
        }

        /// <summary>
        /// Alternative: Configure strongly typed ID converters at the options level
        /// </summary>
        public static IServiceCollection AddExampleDbContextWithExplicitConverters(
            this IServiceCollection services, 
            string connectionString)
        {
            services.AddDbContext<ExampleDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                
                // Explicitly enable strongly typed ID converters
                options.UseStronglyTypedIdConverters();
            });

            return services;
        }
    }
}

/// <summary>
/// Documentation and best practices for strongly typed ID EF Core configuration
/// </summary>
public static class StronglyTypedIdEfCoreBestPractices
{
    /*
    ## Configuration Options (Choose One):

    ### Option 1: Automatic (Recommended)
    ```csharp
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseStronglyTypedIdConverters();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureStronglyTypedIds();
    }
    ```

    ### Option 2: Manual per Property
    ```csharp
    builder.Property(p => p.Id)
        .HasStronglyTypedIdConversion<ProductId, Guid>();
    ```

    ### Option 3: Traditional HasConversion
    ```csharp
    builder.Property(p => p.Id)
        .HasConversion(
            id => id.Value,
            value => ProductId.From(value));
    ```

    ## Database Schema:
    - Strongly typed IDs are stored as their underlying value type
    - ProductId (Guid) → uniqueidentifier in SQL Server
    - CategoryId (int) → int in SQL Server  
    - SkuCode (string) → nvarchar in SQL Server

    ## Performance:
    - No performance overhead - direct value conversion
    - Indexes work normally on the underlying value
    - Foreign keys work as expected

    ## Migrations:
    - EF Core sees the underlying value type for migrations
    - No special migration handling required
    - Column types match the underlying value type

    ## Querying:
    ```csharp
    // All of these work naturally:
    var productId = ProductId.From(someGuid);
    var product = await context.Products.FindAsync(productId);
    var products = await context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
    ```
    */
}