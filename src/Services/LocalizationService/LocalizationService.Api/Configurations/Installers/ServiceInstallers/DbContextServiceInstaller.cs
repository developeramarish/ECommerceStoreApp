﻿using LocalizationService.Api.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LocalizationService.Api.Configurations.Installers.ServiceInstallers;

public class DbContextServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
    {
        var assembly = typeof(Program).Assembly.GetName().Name;

        string defaultConnString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<LocalizationDbContext>(options => options.UseSqlServer(defaultConnString, b => b.MigrationsAssembly(assembly)), ServiceLifetime.Transient);
    }
}
