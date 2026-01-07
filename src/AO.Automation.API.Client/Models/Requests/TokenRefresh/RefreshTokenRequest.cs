using System.Text.Json.Serialization;

namespace AO.Automation.API.Client.Models.Requests.TokenRefresh;

public class RefreshTokenRequest
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
    
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}
