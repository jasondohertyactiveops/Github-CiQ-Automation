namespace AO.Automation.API.Client.Models.Database;

public class UserLoginDetailRecord
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime LoginDateTime { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
    public string SessionValidationToken { get; set; } = string.Empty;
}
