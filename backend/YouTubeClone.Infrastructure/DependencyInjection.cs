using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Infrastructure.Persistence;
using YouTubeClone.Infrastructure.Services;

namespace YouTubeClone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // PostgreSQL
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // UnitOfWork
        services.AddScoped<IUnitOfWork, Persistence.UnitOfWork>();

        // JWT Service
        services.AddScoped<IJwtService, JwtService>();

        // Cloudflare R2
        var r2Config = configuration.GetSection("CloudflareR2");
        var credentials = new BasicAWSCredentials(
            r2Config["AccessKey"],
            r2Config["SecretKey"]);

        var s3Config = new AmazonS3Config
        {
            ServiceURL = r2Config["Endpoint"],
            ForcePathStyle = true
        };

        services.AddSingleton<IAmazonS3>(new AmazonS3Client(credentials, s3Config));
        services.AddScoped<IS3Service, R2Service>();

        // JWT Authentication
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey))
            };
        });

        return services;
    }
}