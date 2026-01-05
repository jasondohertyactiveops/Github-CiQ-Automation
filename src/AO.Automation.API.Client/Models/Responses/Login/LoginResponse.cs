namespace AO.Automation.API.Client.Models.Responses.Login;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int? HomeTeam { get; set; }
    public string Location { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public int? LastSelectedWorkgroupId { get; set; }
    public UserWorkgroupPermissions Permissions { get; set; } = new();
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateTime? WorksFrom { get; set; }
    public DateTime? WorksTo { get; set; }
    public string Username { get; set; } = string.Empty;
}

public class UserWorkgroupPermissions
{
    // Permissions structure - will expand as needed
}
