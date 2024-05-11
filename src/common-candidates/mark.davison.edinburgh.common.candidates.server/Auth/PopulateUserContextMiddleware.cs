namespace mark.davison.edinburgh.common.candidates.server.Auth;

public sealed class PopulateUserContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<ClaimsAppSettings> _claimsSettings;

    public PopulateUserContextMiddleware(
        RequestDelegate next,
        IOptions<ClaimsAppSettings> claimsSettings)
    {
        _next = next;
        _claimsSettings = claimsSettings;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated ?? false)
        {
            var userInfo = context.User.ExtractUserInfo(_claimsSettings.Value);

            if (userInfo == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Could not extract id claim");
                return;
            }

            var userContext = context.RequestServices.GetRequiredService<ICurrentUserContext>();

            userContext.CurrentUser = new()
            {
                Id = userInfo.Id,
                Sub = userInfo.Id, // TODO: sub vs id
                First = userInfo.First,
                Last = userInfo.Last,
                Username = userInfo.Username,
                Email = userInfo.Email,
                Admin = false
            };
        }

        await _next(context);
    }
}