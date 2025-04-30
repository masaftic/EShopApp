using EShopApp.Api;
using EShopApp.Application;
using EShopApp.Infrastructure;
using EShopApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Logging.AddConsole();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddPresentation();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}


var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseExceptionHandler("/error-development");
    }
    else
    {
        app.UseExceptionHandler("/error");
    }
    
    // app.UseHttpsRedirection();
    app.UseCors("AllowMyClient");
    app.UseRateLimiter();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
}

app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("{Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.MigrateAsync();
    if (!app.Environment.IsEnvironment("Testing")) // Skip seeding in testing
    {
        await seeder.SetUpRoles();
        await seeder.SeedAsync();
    }
}

app.Run();


public partial class Program { }
