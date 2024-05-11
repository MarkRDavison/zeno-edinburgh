using mark.davison.common.CQRS;
using mark.davison.common.server.abstractions.Authentication;
using mark.davison.common.server.abstractions.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace mark.davison.edinburgh.bff.web;

public class Startup
{
    public IConfiguration Configuration { get; }
    public AppSettings AppSettings { get; set; } = null!;

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
            .UseCookieOidcAuth(AppSettings.AUTH)
            .ConfigureHealthCheckServices()
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
        //.AddReverseProxy();

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

        app.UseHttpsRedirection();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app
            .UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseMiddleware<CheckAccessTokenValidityMiddleware>()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .MapHealthChecks();

                MapProxyCQRSGet(endpoints, "/api/startup-query");
                endpoints
                    //.UseApiProxy(AppSettings.API_ORIGIN)
                    .UseAuthEndpoints(AppSettings.WEB_ORIGIN);
            });
    }
    static void MapProxyCQRSGet(IEndpointRouteBuilder endpoints, string path)
    {
        endpoints.MapGet(
            path,
            async (HttpContext context, [FromServices] IOptions<AppSettings> options, [FromServices] IHttpClientFactory httpClientFactory, [FromServices] ICurrentUserContext currentUserContext, CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrEmpty(currentUserContext.Token))
                {
                    return Results.Unauthorized();
                }

                var client = httpClientFactory.CreateClient("PROXY");

                var headers = HeaderParameters.Auth(currentUserContext.Token, null);

                var request = new HttpRequestMessage(HttpMethod.Get, $"{options.Value.API_ORIGIN.TrimEnd('/')}{path}{context.Request.QueryString}");

                foreach (var k in headers)
                {
                    request.Headers.Add(k.Key, k.Value);
                }

                var response = await client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Results.Text(content);
                }

                Console.WriteLine(await response.Content.ReadAsStringAsync());

                return Results.BadRequest(new Response
                {
                    Errors = new() { $"Error: {response.StatusCode}" }
                });
            });
    }
}
