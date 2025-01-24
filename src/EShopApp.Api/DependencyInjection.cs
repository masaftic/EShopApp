using System.Text.Json.Serialization;
using ErrorOr;

namespace EShopApp.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers(); 
        services.AddProblemDetails(config =>
        {
            config.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                if (context.HttpContext.Items["errors"] is List<Error> errors)
                {
                    context.ProblemDetails.Extensions.TryAdd("errorsCodes", errors.Select(e => e.Code).ToArray());
                }
            };
        });


        return services;
    }
}