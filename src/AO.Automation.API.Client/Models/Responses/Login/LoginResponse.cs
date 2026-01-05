using System.Text.Json.Serialization;

namespace AO.Automation.API.Client.Models.Responses.Login;

public class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
    
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
    
    [JsonPropertyName("homeTeam")]
    public int? HomeTeam { get; set; }
    
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;
    
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;
    
    [JsonPropertyName("lastSelectedWorkgroupId")]
    public int? LastSelectedWorkgroupId { get; set; }
    
    [JsonPropertyName("permissions")]
    public UserWorkgroupPermissions Permissions { get; set; } = new();
    
    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }
    
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;
    
    [JsonPropertyName("worksFrom")]
    public DateTime? WorksFrom { get; set; }
    
    [JsonPropertyName("worksTo")]
    public DateTime? WorksTo { get; set; }
    
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
}

public class UserWorkgroupPermissions
{
    // Permissions structure - will expand as needed
}
