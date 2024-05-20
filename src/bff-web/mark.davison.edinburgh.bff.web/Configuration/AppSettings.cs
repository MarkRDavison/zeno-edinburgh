namespace mark.davison.edinburgh.bff.web.Configuration;

public sealed class AppSettings : IAppSettings
{
    public string SECTION => "EDINBURGH";

    public AuthAppSettings AUTH { get; set; } = new();
    public RedisAppSettings REDIS { get; set; } = new();
    public ClaimsAppSettings2 CLAIMS2 { get; set; } = new();
    public string WEB_ORIGIN { get; set; } = "https://localhost:8080";
    public string BFF_ORIGIN { get; set; } = "https://localhost:40000";
    public string API_ORIGIN { get; set; } = "https://localhost:50000";
    public bool PRODUCTION_MODE { get; set; }
}
