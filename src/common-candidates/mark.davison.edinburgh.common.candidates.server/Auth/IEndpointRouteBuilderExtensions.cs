﻿namespace mark.davison.edinburgh.common.candidates.server.Auth;

public static class IEndpointRouteBuilderExtensions
{

    public static IEndpointRouteBuilder UseApiProxy(
        this IEndpointRouteBuilder endpoints,
        string apiEndpoint)
    {
        endpoints.Map("/api/{**catchall}", async (
            HttpContext context,
            [FromServices] IHttpClientFactory httpClientFactory,
            CancellationToken cancellationToken) =>
        {
            var access = await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (string.IsNullOrEmpty(access))
            {
                return Results.Unauthorized();
            }

            var client = httpClientFactory.CreateClient("ApiProxy");
            var request = new HttpRequestMessage(
            new HttpMethod(context.Request.Method),
                $"{apiEndpoint.TrimEnd('/')}{context.Request.Path}{context.Request.QueryString}");

            foreach (var (key, headers) in context.Request.Headers)
            {
                foreach (var h in headers)
                {
                    request.Headers.TryAddWithoutValidation(key, h);
                }
            }

            var response = await client.SendAsync(request, cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Results.Text(content);
            }

            Console.WriteLine(content);

            return Results.BadRequest(new Response
            {
                Errors = [$"{response.StatusCode}", content]
            });
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
