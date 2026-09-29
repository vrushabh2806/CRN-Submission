using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Application.Mapping;
using ProductsAPI.Application.Services;

namespace ProductsAPI.Application;

/// <summary>Extension methods for registering Application layer services.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // FluentValidation — scan all validators in this assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IItemService, ItemService>();

        return services;
    }
}
