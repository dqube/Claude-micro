using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;
using PromotionService.Infrastructure.Persistence;
using PromotionService.Infrastructure.Repositories;

namespace PromotionService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Database Context
        services.AddDbContext<PromotionDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // For development, use InMemory database for simplicity
                options.UseInMemoryDatabase("PromotionServiceDb");
            }
        });
        
        // Register PromotionDbContext as IDbContext for BuildingBlocks services
        services.AddScoped<BuildingBlocks.Infrastructure.Data.Context.IDbContext, PromotionDbContext>();

        // Register DiscountType Repository
        services.AddScoped<IDiscountTypeRepository, DiscountTypeRepository>();
        services.AddScoped<IRepository<DiscountType, DiscountTypeId>, DiscountTypeRepository>();
        services.AddScoped<IReadOnlyRepository<DiscountType, DiscountTypeId>, DiscountTypeRepository>();

        // Register DiscountCampaign Repository
        services.AddScoped<IDiscountCampaignRepository, DiscountCampaignRepository>();
        services.AddScoped<IRepository<DiscountCampaign, CampaignId>, DiscountCampaignRepository>();
        services.AddScoped<IReadOnlyRepository<DiscountCampaign, CampaignId>, DiscountCampaignRepository>();

        // Register DiscountRule Repository
        services.AddScoped<IDiscountRuleRepository, DiscountRuleRepository>();
        services.AddScoped<IRepository<DiscountRule, RuleId>, DiscountRuleRepository>();
        services.AddScoped<IReadOnlyRepository<DiscountRule, RuleId>, DiscountRuleRepository>();

        // Register Promotion Repository
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IRepository<Promotion, PromotionId>, PromotionRepository>();
        services.AddScoped<IReadOnlyRepository<Promotion, PromotionId>, PromotionRepository>();

        // Register PromotionProduct Repository
        services.AddScoped<IPromotionProductRepository, PromotionProductRepository>();
        services.AddScoped<IRepository<PromotionProduct, PromotionProductId>, PromotionProductRepository>();
        services.AddScoped<IReadOnlyRepository<PromotionProduct, PromotionProductId>, PromotionProductRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register BuildingBlocks OutboxService instead of custom implementation
        services.AddScoped<BuildingBlocks.Application.Outbox.IOutboxService, BuildingBlocks.Infrastructure.Services.OutboxService>();

        return services;
    }
} 