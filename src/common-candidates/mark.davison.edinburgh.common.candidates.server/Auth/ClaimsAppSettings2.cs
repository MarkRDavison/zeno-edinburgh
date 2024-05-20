namespace mark.davison.edinburgh.common.candidates.server.Auth;

public sealed class ClaimsAppSettings2 : IAppSettings
{
    public string SECTION => "CLAIMS2";

    public string OIDC_ID_ATTRIBUTE { get; set; } = string.Empty;
    public string OIDC_EMAIL_ATTRIBUTE { get; set; } = string.Empty;
    public string OIDC_FIRST_NAME_ATTRIBUTE { get; set; } = string.Empty;
    public string OIDC_LAST_NAME_ATTRIBUTE { get; set; } = string.Empty;
    public string OIDC_USERNAME_ATTRIBUTE { get; set; } = string.Empty;
}
