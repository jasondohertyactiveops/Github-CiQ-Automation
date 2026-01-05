namespace AO.Automation.API.Client.Models;

/// <summary>
/// Generic wrapper for API responses
/// </summary>
public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;
}
