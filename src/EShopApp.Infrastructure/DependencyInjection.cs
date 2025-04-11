using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Common.Options;
using EShopApp.Infrastructure.Authentication;
using EShopApp.Infrastructure.Data;
using EShopApp.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Stripe;
using EShopApp.Infrastructure.Identity;
using EShopApp.Infrastructure.Payment;
using EShopApp.Infrastructure.Data.Identity;
using Amazon.S3;
using Dumpify;
using EShopApp.Infrastructure.ImageStorage;

namespace EShopApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var stripeApiCredentials = configuration.GetSection(StripeApiCredentials.SectionName).Get<StripeApiCredentials>() ?? throw new ArgumentNullException("Stripe API credentials not found in configuration.");

        StripeConfiguration.ApiKey = stripeApiCredentials.SecretKey;

        services.Configure<StripeApiCredentials>(configuration.GetSection(StripeApiCredentials.SectionName));

        AddServices(services, configuration);
        AddPersistence(services, configuration);
        AddAuth(services, configuration);

        return services;
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)

    {
        services.AddScoped<IIdentityService, Identity.IdentityService>();
        services.AddScoped<DataSeeder>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPaymentService, StripePaymentService>();
        services.AddHttpContextAccessor();
        services.AddHostedService<ExpiredReservationBackgroundService>();
        services.AddHostedService<ExpiredRefreshTokensBackgroundService>();

        services.Configure<S3Settings>(configuration.GetSection("S3Settings"));
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
            var amazonS3Config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(s3Settings.Region),
                HttpClientFactory = new AmazonS3HttpClientFactory(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                })
            };

            var credentials = new Amazon.Runtime.BasicAWSCredentials(s3Settings.AccessKey, s3Settings.SecretKey);



            return new AmazonS3Client(credentials, amazonS3Config);
        });
        services.AddScoped<IImageStorageService, S3ImageStorageService>();
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DockerConnection"));
        });

        var identityOptions = new ApplicationIdentityOptions();

        services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = identityOptions.PasswordRequiredLength;
                options.Password.RequireDigit = identityOptions.PasswordRequireDigit;
                options.Password.RequireLowercase = identityOptions.PasswordRequireLowercase;
                options.Password.RequireUppercase = identityOptions.PasswordRequireUppercase;
                options.Password.RequireNonAlphanumeric = identityOptions.PasswordRequireNonAlphanumeric;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddSingleton(identityOptions);
    }

    private static void AddAuth(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthorization();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully.");
                        return Task.CompletedTask;
                    }
                };
            });


        // Suppress the default cookie scheme added by Identity
        // services.ConfigureApplicationCookie(options =>
        // {
        //     options.Cookie.HttpOnly = true;
        //     options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        //     options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        //     options.SlidingExpiration = true;
        //     options.LoginPath = string.Empty; // Prevent redirects for unauthorized access
        // });
    }
}
