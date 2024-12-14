using EShopApp.Application.Common.Helpers;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EShopApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config => 
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<CategoryPathProcessor>();
        
        return services;
    }
}