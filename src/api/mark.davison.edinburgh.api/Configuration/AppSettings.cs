namespace mark.davison.edinburgh.api.Configuration;

public sealed class AppSettings : IAppSettings
{
    public string SECTION => "EDINBURGH";

    public AuthAppSettings AUTH { get; set; } = new();
    public DatabaseAppSettings DATABASE { get; set; } = new();
    public RedisAppSettings REDIS { get; set; } = new();
    public ClaimsAppSettings2 CLAIMS { get; set; } = new();
    public bool PRODUCTION_MODE { get; set; }
}
