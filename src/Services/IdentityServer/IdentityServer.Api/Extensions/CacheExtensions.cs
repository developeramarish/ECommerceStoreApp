﻿using IdentityServer.Api.Dtos.Localization;
using IdentityServer.Api.Models.Base.Concrete;
using IdentityServer.Api.Models.CacheModels;
using IdentityServer.Api.Models.Settings;
using IdentityServer.Api.Services.Localization.Abstract;
using IdentityServer.Api.Services.Redis.Abstract;
using IdentityServer.Api.Utilities.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Serilog;
using StackExchange.Redis;
using System.Reflection;

namespace IdentityServer.Api.Extensions
{
    public static class CacheExtensions
    {
        public static T CacheOrGet<T>(this StackExchange.Redis.IDatabase db,
                               string key,
                               int duration,
                               Func<T> filter) where T : class
        {
            if (!string.IsNullOrWhiteSpace(db.StringGet(key)))
            {
                var value = db.StringGet(key);
                return value.ToString().ToObject<T>();
            }
            else
            {
                var result = filter();

                if (result != null)
                {
                    string serializedValue = JsonConvert.SerializeObject(result, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                    db.StringSet(key, serializedValue, TimeSpan.FromMinutes(duration));
                }

                return result;
            }
        }

        public static async Task<T> CacheOrGetAsync<T>(this StackExchange.Redis.IDatabase db,
                               string key,
                               int duration,
                               Func<T> filter) where T : class
        {
            if (!string.IsNullOrWhiteSpace(await db.StringGetAsync(key)))
            {
                var value = await db.StringGetAsync(key);
                return value.ToString().ToObject<T>();
            }
            else
            {
                var result = filter();

                if (result != null)
                {
                    string serializedValue = JsonConvert.SerializeObject(result, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                    await db.StringSetAsync(key, serializedValue, TimeSpan.FromMinutes(duration));
                }

                return result;
            }
        }

        public static string GetCacheKeyByModel(CacheKeyModel model)
        {
            var parameters = string.Join("-", model.Parameters);

            var result = string.Join("-",
                             model.Prefix,
                             model.ProjectName,
                             model.ClassName,
                             model.MethodName,
                             model.Language,
                             parameters);

            return result;
        }

        public static string GetCacheKey(string[] parameters, string prefix = "")
        {
            var values = string.Join("-", parameters);

            var result = string.Join("-", prefix, values);
            return result;
        }

        public static async Task LocalizationCacheInitialize(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

            var policy = Polly.Policy.Handle<Exception>()
                        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                        {
                            Log.Error("ERROR handling message: {ExceptionMessage} - Method : {ClassName}.{MethodName}",
                                                ex.Message, nameof(CacheExtensions),
                                                MethodBase.GetCurrentMethod()?.Name);
                        });

            await policy.ExecuteAsync(async () =>
            {
                var values = new Dictionary<string, RedisValue>();

                var localizationSettings = configuration.GetSection("LocalizationSettings").Get<LocalizationSettings>();
                var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>();

                var localizationMemberKey = localizationSettings.MemberKey;
                var redisCacheDuration = localizationSettings.CacheDuration;

                var localizationSuffix1 = localizationSettings.MemoryCache.Suffix1;
                var localizationSuffix2 = localizationSettings.MemoryCache.Suffix2;

                var memoryCache1Prefix = $"{localizationMemberKey}-{localizationSuffix1}";
                var memoryCache2Prefix = $"{localizationMemberKey}-{localizationSuffix2}";

                int databaseId = redisSettings.LocalizationCacheDbId;

                if (!redisService.AnyKeyExistsByPrefix(localizationMemberKey, databaseId) ||
                     !memoryCache.TryGetValue(memoryCache1Prefix, out object cacheDummy1) || 
                     !memoryCache.TryGetValue(memoryCache2Prefix, out object cacheDummy2))
                {
                    var gatewayClient = httpClientFactory.CreateClient("gateway-specific");
                    var result = await gatewayClient.PostGetResponseAsync<DataResult<MemberDto>, StringModel>("localization/members/get-with-resources-by-memberkey-and-save", new StringModel() { Value = localizationMemberKey });

                    if (!result.Success)
                        throw new Exception("Localization data request not successful");

                    var resultData = result.Data;
                    if (resultData == null)
                        throw new Exception("Localization data is null from http request");

                    MemoryCacheExtensions.SaveLocalizationData(memoryCache, configuration, resultData);

                    if (redisService.AnyKeyExistsByPrefix(localizationMemberKey, databaseId))
                        return;

                    foreach (var resource in resultData.Resources)
                    {
                        await redisService.SetAsync($"{localizationMemberKey}-{resource.LanguageCode}-{resource.Tag}", resource, redisCacheDuration, databaseId);
                    }
                }
            });
        }
    }
}