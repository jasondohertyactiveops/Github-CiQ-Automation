using System.Text.Json.Serialization;

namespace AO.Automation.API.Client.Models.Responses.TokenRefresh;

public class TokenRefreshResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
    
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}
