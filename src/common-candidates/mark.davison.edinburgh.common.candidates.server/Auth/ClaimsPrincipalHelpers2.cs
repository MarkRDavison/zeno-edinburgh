namespace mark.davison.edinburgh.common.candidates.server.Auth;

public static class ClaimsPrincipalHelpers2
{
    public static UserInfo? ExtractUserInfo(this ClaimsPrincipal claimsPrincipal, ClaimsAppSettings2 claimsSettings)
    {
        var idClaimValue = claimsPrincipal.Claims.FirstOrDefault(_ => _.Type == claimsSettings.OIDC_ID_ATTRIBUTE);

        if (!Guid.TryParse(idClaimValue?.Value, out Guid id))
        {
            return null;
        }

        var firstNameClaimValue = claimsPrincipal.Claims.FirstOrDefault(_ => _.Type == claimsSettings.OIDC_FIRST_NAME_ATTRIBUTE)?.Value ?? string.Empty;
        var lastNameClaimValue = claimsPrincipal.Claims.FirstOrDefault(_ => _.Type == claimsSettings.OIDC_LAST_NAME_ATTRIBUTE)?.Value ?? string.Empty;
        var usernameClaimValue = claimsPrincipal.Claims.FirstOrDefault(_ => _.Type == claimsSettings.OIDC_USERNAME_ATTRIBUTE)?.Value ?? string.Empty;
        var emailClaimValue = claimsPrincipal.Claims.FirstOrDefault(_ => _.Type == claimsSettings.OIDC_EMAIL_ATTRIBUTE)?.Value ?? string.Empty;

        return new UserInfo // TODO: Consolidate this with UserProfile etc
        {
            Id = id,
            Sub = id,
            First = firstNameClaimValue,
            given_name = firstNameClaimValue,
            Last = lastNameClaimValue,
            family_name = lastNameClaimValue,
            Name = $"{firstNameClaimValue} {lastNameClaimValue}",
            Username = usernameClaimValue,
            Email = emailClaimValue
        };
    }
}
