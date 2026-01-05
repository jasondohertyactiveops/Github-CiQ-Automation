namespace AO.Automation.API.Client.Models.Database;

public class UserRecord
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int ActivationStatusId { get; set; }
    public bool Active { get; set; }
    public string? PasswordHash { get; set; }
    public int StaffMemberId { get; set; }
}
