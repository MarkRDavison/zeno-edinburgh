namespace mark.davison.edinburgh.common.candidates.server.Auth;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddJwtAuth2(
        this IServiceCollection services,
        AuthAppSettings authAppSettings)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authAppSettings.AUTHORITY,
                    ValidateAudience = !string.IsNullOrEmpty(authAppSettings.CLIENT_ID /*TODO AUDIENCE vs CLIENT_ID*/),
                    ValidAudience = authAppSettings.CLIENT_ID /*TODO AUDIENCE vs CLIENT_ID*/,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    ClockSkew = TimeSpan.Zero,
                    SignatureValidator = (token, _) => new JsonWebToken(token),
                    RequireExpirationTime = true,
                };
            });

        return services;
    }

    public static IServiceCollection UseCookieOidcAuth2(
        this IServiceCollection services,
        AuthAppSettings authAppSettings)
    {
        services
            .AddAuthentication(_ =>
            {
                _.DefaultScheme = AuthConstants2.CookiesScheme;
                _.DefaultChallengeScheme = AuthConstants2.OidcScheme;
                _.DefaultSignOutScheme = AuthConstants2.OidcScheme;
            })
            .AddOpenIdConnect(AuthConstants2.OidcScheme, _ =>
            {
                _.Authority = authAppSettings.AUTHORITY;
                _.ClientId = authAppSettings.CLIENT_ID;
                _.ClientSecret = authAppSettings.CLIENT_SECRET;

                _.TokenValidationParameters = new()
                {
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(0)
                };

                _.Events = new()
                {
                    OnRedirectToIdentityProvider = (ctx) =>
                    {
                        if (ctx.ProtocolMessage.RedirectUri.StartsWith(AuthConstants2.HttpProto))
                        {
                            ctx.ProtocolMessage.RedirectUri = ctx.ProtocolMessage.RedirectUri.Replace(AuthConstants2.HttpProto, AuthConstants2.HttpsProto);
                        }

                        return Task.CompletedTask;
                    }
                };
                _.ResponseType = OpenIdConnectResponseType.Code;
                _.GetClaimsFromUserInfoEndpoint = true;
                _.SaveTokens = true;
                _.UsePkce = true;
                _.Scope.Clear();

                foreach (var scope in authAppSettings.SCOPES)
                {
                    _.Scope.Add(scope);
                }
            })
            .AddCookie(AuthConstants2.CookiesScheme, _ =>
            {
                _.ExpireTimeSpan = TimeSpan.FromHours(8);
                _.SlidingExpiration = false;
                _.AccessDeniedPath = "/failedcookie";
                _.Cookie.Name = $"__{authAppSettings.SESSION_NAME}".ToLower();
                _.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                _.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                _.Cookie.IsEssential = false;
                _.LogoutPath = AuthConstants2.LogoutPath;
                _.LoginPath = AuthConstants2.LoginPath;

                _.Events = new()
                {
                    OnSignedIn = async (ctx) =>
                    {
                        await Task.CompletedTask;
                    }
                };
            });

        services
            .AddHttpClient(AuthConstants2.AuthHttpClient);

        return services;
    }
}
