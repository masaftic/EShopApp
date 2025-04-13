using System.Reflection;
using EShopApp.Application.Common.Behaviors;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Orders.Services;
using EShopApp.Application.Payments.Services;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EShopApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        
        services.AddScoped<IOrderService, OrderService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        services.AddMappings();
        
        return services;
    }


    private static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly()); // Registers all mapping classes : IRegister
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}