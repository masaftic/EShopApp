using EShopApp.Application.Common.Behaviors;
using EShopApp.Application.Common.Helpers;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EShopApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config => 
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<CategoryPathProcessor>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        return services;
    }
}