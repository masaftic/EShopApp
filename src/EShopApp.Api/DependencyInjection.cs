using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShopApp.Infrastructure.Identity;
using EShopApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace EShopApp.Api;


public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();

        return services;
    }
}