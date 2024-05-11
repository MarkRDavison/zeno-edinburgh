namespace mark.davison.edinburgh.common.candidates.shared.Auth;

public class UserInfo
{
    public Guid Id { get; set; }
    public Guid Sub { get; set; }
    public string First { get; set; } = string.Empty;
    public string given_name { get; set; } = string.Empty;
    public string Last { get; set; } = string.Empty;
    public string family_name { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
