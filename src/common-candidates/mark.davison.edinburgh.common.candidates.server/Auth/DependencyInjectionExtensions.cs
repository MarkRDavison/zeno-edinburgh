namespace mark.davison.edinburgh.common.candidates.server.Auth;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddJwtAuth(
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

    public static IServiceCollection UseCookieOidcAuth(
        this IServiceCollection services,
        AuthAppSettings authAppSettings)
    {
        services
            .AddAuthentication(_ =>
            {
                _.DefaultScheme = AuthConstants.CookiesScheme;
                _.DefaultChallengeScheme = AuthConstants.OidcScheme;
                _.DefaultSignOutScheme = AuthConstants.OidcScheme;
            })
            .AddOpenIdConnect(AuthConstants.OidcScheme, _ =>
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
                        if (ctx.ProtocolMessage.RedirectUri.StartsWith(AuthConstants.HttpProto))
                        {
                            ctx.ProtocolMessage.RedirectUri = ctx.ProtocolMessage.RedirectUri.Replace(AuthConstants.HttpProto, AuthConstants.HttpsProto);
                        }

                        return Task.CompletedTask;
                    }
                };

                _.ResponseType = OpenIdConnectResponseType.Code;
                _.GetClaimsFromUserInfoEndpoint = true;
                _.SaveTokens = true;
                _.Scope.Clear();

                foreach (var scope in authAppSettings.SCOPES)
                {
                    _.Scope.Add(scope);
                }
            })
            .AddCookie(AuthConstants.CookiesScheme, _ =>
            {
                _.ExpireTimeSpan = TimeSpan.FromHours(8);
                _.SlidingExpiration = false;
                _.Cookie.Name = $"__{authAppSettings.SESSION_NAME}".ToLower();
                _.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                _.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                _.LogoutPath = AuthConstants.LogoutPath;
                _.LoginPath = AuthConstants.LoginPath;
            });

        services
            .AddHttpClient(AuthConstants.AuthHttpClient);

        return services;
    }
}
