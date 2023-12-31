﻿using CareerCompassAPI.Application.Abstraction.Services;
using CareerCompassAPI.Application.Abstraction.Storage;
using CareerCompassAPI.Application.Abstraction.Storage.Azure;
using CareerCompassAPI.Infrastructure.Services.Azure;
using CareerCompassAPI.Infrastructure.Services.Token;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CareerCompassAPI.Infrastructure.Services
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtService,JwtService>();
            services.AddScoped<IStorageService,StorageService>();
            services.AddScoped<IAzureStorage,AzureStorage>();
            services.AddLogging(builder => {
                builder.AddConsole();
            });
     
        }
        public static void AddStorage<T>(this  IServiceCollection services) where T : class , IStorage
        {
            services.AddScoped<IStorage, T>();
        }
        public static void AddSignalRServices(this IServiceCollection services)
        {
            services.AddSignalR();
        }
    }
}
