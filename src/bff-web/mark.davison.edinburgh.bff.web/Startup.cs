﻿namespace mark.davison.edinburgh.bff.web;

public class Startup
{
    public IConfiguration Configuration { get; }
    public AppSettings AppSettings { get; set; } = default!;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AppSettings = services.ConfigureSettingsServices<AppSettings>(Configuration);
        if (AppSettings == null) { throw new InvalidOperationException(); }

        Console.WriteLine(AppSettings.DumpAppSettings(AppSettings.PRODUCTION_MODE));

        services
            .AddCors()
            .UseCookieOidcAuth2(AppSettings.AUTH)
            .AddHealthCheckServices()
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(builder =>
            builder
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .SetIsOriginAllowed(_ => true) // TODO: Config driven
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader());

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app
            .UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            })
            .UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseMiddleware<CheckAccessTokenValidityMiddleware2>()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .UseApiProxy2(AppSettings.API_ORIGIN)
                    .UseAuthEndpoints2(AppSettings.WEB_ORIGIN)
                    .MapHealthChecks();
            });
    }
}
