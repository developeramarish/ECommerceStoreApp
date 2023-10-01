﻿using MonitoringService.Api.Extensions;
using MonitoringService.Api.Services.Localization.Abstract;
using System.Reflection;

namespace MonitoringService.Api.Services.Base;

public abstract class BaseService
{
    private readonly ILocalizationService _localizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<BaseService> _logger;

    protected BaseService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        if (httpContextAccessor == null)
            Serilog.Log.Error("ERROR handling message: {ExceptionMessage} - Method : {ClassName}",
                       $"{nameof(HttpContextAccessor)} is null",
                       nameof(BaseService));

        _localizationService = httpContextAccessor?.HttpContext?.RequestServices?.GetRequiredService<ILocalizationService>();
        _logger = httpContextAccessor?.HttpContext?.RequestServices?.GetRequiredService<ILogger<BaseService>>();
        Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }

    public string Env { get; set; }

    public string GetLocalizedValue(string key)
    {
        try
        {
            string culture = HttpExtensions.GetAcceptLanguage(_httpContextAccessor);
            var result = _localizationService.GetStringResource(culture, key);
            return result;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error("ERROR handling message: {ExceptionMessage} - Method : {ClassName}.{MethodName} - Key : {Key}",
                       ex.Message,
                       nameof(BaseService),
                       MethodBase.GetCurrentMethod()?.Name,
                       key);

            return "Error";
        }
    }
}
