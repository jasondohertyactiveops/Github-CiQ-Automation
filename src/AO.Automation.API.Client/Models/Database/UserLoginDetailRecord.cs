namespace AO.Automation.API.Client.Models.Database;

public class UserLoginDetailRecord
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
    public DateTime Created { get; set; }
}
