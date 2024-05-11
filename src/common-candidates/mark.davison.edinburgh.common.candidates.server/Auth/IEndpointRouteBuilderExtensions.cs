namespace mark.davison.edinburgh.common.candidates.server.Auth;

public static class IEndpointRouteBuilderExtensions
{

    public static IEndpointRouteBuilder UseApiProxy(
        this IEndpointRouteBuilder endpoints,
        string apiEndpoint)
    {
        var transformer = HttpTransformer.Default;
        var requestConfig = new ForwarderRequestConfig
        {
            ActivityTimeout = TimeSpan.FromSeconds(100)
        };
        var httpClient = new HttpMessageInvoker(new SocketsHttpHandler
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
            EnableMultipleHttp2Connections = true,
            ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
            ConnectTimeout = TimeSpan.FromSeconds(15),
        });

        endpoints
            .Map("/api/{*rest}", async (HttpContext context, [FromServices] IHttpForwarder forwarder, [FromServices] ILoggerFactory loggerFactory) =>
            {
                var error = await forwarder
                .SendAsync(
                    context,
                    apiEndpoint,
                    httpClient,
                    requestConfig,
                    transformer);

                if (error != ForwarderError.None)
                {
                    var errorFeature = context.GetForwarderErrorFeature();
                    var exception = errorFeature?.Exception;

                    if (exception != null)
                    {
                        var logger = loggerFactory.CreateLogger("YARP");

                        logger.LogError(exception.Message);
                        logger.LogError(exception.StackTrace);
                    }
                }
            })
            .RequireAuthorization();

        return endpoints;
    }

    public static IEndpointRouteBuilder UseAuthEndpoints(
        this IEndpointRouteBuilder endpoints,
        string webOrigin)
    {
        endpoints
            .MapGet("/auth/user", (HttpContext context, [FromServices] IOptions<ClaimsAppSettings> claimsSettings) =>
            {
                if (context.User?.Identity?.IsAuthenticated ?? false)
                {
                    if (context.User.ExtractUserInfo(claimsSettings.Value) is var userInfo)
                    {
                        return Results.Ok(userInfo);
                    }
                }

                return Results.Unauthorized();
            })
            .AllowAnonymous();

        endpoints
            .MapGet(AuthConstants.LoginPath, async (HttpContext context, [FromQuery(Name = "returnUrl")] string? returnUrl) =>
            {
                var prop = new AuthenticationProperties
                {
                    RedirectUri = returnUrl ?? webOrigin.TrimEnd('/') + AuthConstants.LoginCompletePath
                };

                await context.ChallengeAsync(prop);
            })
            .AllowAnonymous();

        endpoints
            .MapGet(AuthConstants.LogoutPath, async (HttpContext context) =>
            {
                await context.SignOutAsync(AuthConstants.CookiesScheme);
                var prop = new AuthenticationProperties
                {
                    RedirectUri = webOrigin.TrimEnd('/') + AuthConstants.LogoutCompletePath
                };
                await context.SignOutAsync(AuthConstants.OidcScheme, prop);
            })
            .RequireAuthorization();

        return endpoints;
    }
}
