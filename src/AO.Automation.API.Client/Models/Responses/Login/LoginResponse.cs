namespace AO.Automation.API.Client.Models.Responses.Login;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserInfo User { get; set; } = new();
}

public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int StaffMemberId { get; set; }
    public string Name { get; set; } = string.Empty;
}
