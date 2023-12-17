﻿
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace CatalogService.Api.Configurations.Installers.WebHostBuilderInstallers;

public class GrpcWebHostBuilderInstaller : IWebHostBuilderInstaller
{
    public void Install(ConfigureWebHostBuilder builder, IWebHostEnvironment hostEnv, IConfiguration configuration)
    {
        if (hostEnv.IsProduction())
        {
            builder.UseKestrel(options =>
            {
                var ports = GetDefinedPorts(configuration);
                options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
                options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            });
        }
        else
        {
            builder.UseKestrel(options =>
            {
                var ports = GetDefinedPorts(configuration);
                options.ListenLocalhost(ports.grpcPort, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
                options.ListenLocalhost(ports.httpPort, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            });
        }
    }

    (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
    {
        var grpcPort = config.GetValue("GRPC_PORT", 7006);
        var port = config.GetValue("PORT", 5006);
        return (port, grpcPort);
    }
}
