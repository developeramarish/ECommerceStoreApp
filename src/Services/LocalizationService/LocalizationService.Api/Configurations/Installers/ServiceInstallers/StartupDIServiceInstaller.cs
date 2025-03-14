﻿using LocalizationService.Api.Models.LogModels;
using LocalizationService.Api.Services.ElasticSearch.Abstract;
using LocalizationService.Api.Services.ElasticSearch.Concrete;
using LocalizationService.Api.Services.Redis.Abstract;
using LocalizationService.Api.Services.Redis.Concrete;

namespace LocalizationService.Api.Configurations.Installers.ServiceInstallers;

public class StartupDIServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
    {
        services.AddSingleton<IElasticSearchService, ElasticSearchService>();
        services.AddSingleton<IRedisService, RedisService>();

        this.CreateELKIndex(services, configuration);
    }

    private async void CreateELKIndex(IServiceCollection services, IConfiguration configuration)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var elasticSearchService = scope.ServiceProvider.GetRequiredService<IElasticSearchService>();

        var elasticLogOptions = configuration.GetSection("ElasticSearchOptions").Get<ElasticSearchOptions>();
        await elasticSearchService.CreateIndexAsync<LogDetail>(elasticLogOptions.LogIndex);
    }
}
