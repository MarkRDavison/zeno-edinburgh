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
            .UseMiddleware<CheckAccessTokenValidityMiddleware>()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .MapHealthChecks();

                MapProxyCQRSGet(endpoints, "/api/startup-query");

                // TODO: Works but seems slow?
                //endpoints.Map("/api/{**catchall}", async (HttpContext context, [FromServices] IHttpClientFactory httpClientFactory) =>
                //{
                //    var client = httpClientFactory.CreateClient("PROXY");
                //    var targetUri = new Uri(AppSettings.API_ORIGIN + context.Request.Path);
                //
                //    var targetRequestMessage = new HttpRequestMessage();
                //    targetRequestMessage.RequestUri = targetUri;
                //    targetRequestMessage.Method = new HttpMethod(context.Request.Method);
                //
                //    // Copying the headers from the incoming request to the target request
                //    foreach (var header in context.Request.Headers)
                //    {
                //        targetRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                //    }
                //
                //    // Send the request to the target server
                //    var responseMessage = await client.SendAsync(targetRequestMessage);
                //
                //    // Copy the response back to the original client
                //    context.Response.StatusCode = (int)responseMessage.StatusCode;
                //    foreach (var header in responseMessage.Headers)
                //    {
                //        context.Response.Headers[header.Key] = header.Value.ToArray();
                //    }
                //
                //    await responseMessage.Content.CopyToAsync(context.Response.Body);
                //});

                endpoints
                    //.UseApiProxy(AppSettings.API_ORIGIN)
                    .UseAuthEndpoints(AppSettings.WEB_ORIGIN);
            });
    }
    static void MapProxyCQRSGet(IEndpointRouteBuilder endpoints, string path)
    {
        endpoints.MapGet(
            path,
            async (HttpContext context, [FromServices] IOptions<AppSettings> options, [FromServices] IHttpClientFactory httpClientFactory, CancellationToken cancellationToken) =>
            {
                var access = await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                if (string.IsNullOrEmpty(access))
                {
                    return Results.Unauthorized();
                }

                var client = httpClientFactory.CreateClient("PROXY");

                var headers = HeaderParameters.Auth(access, null);

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
