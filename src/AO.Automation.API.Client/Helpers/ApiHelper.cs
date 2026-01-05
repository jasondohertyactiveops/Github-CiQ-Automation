using System.Net.Http.Json;
using AO.Automation.API.Client.Models;

namespace AO.Automation.API.Client.Helpers;

/// <summary>
/// HTTP client wrapper for API requests
/// </summary>
public class ApiHelper
{
    private readonly HttpClient _httpClient;
    
    public ApiHelper()
    {
        var baseUrl = ApiTestConfig.Instance.ApiBaseUrl;
        _httpClient = new HttpClient 
        { 
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }
    
    /// <summary>
    /// Set authentication token for subsequent requests
    /// </summary>
    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
    
    /// <summary>
    /// Generic POST request
    /// </summary>
    public async Task<ApiResponse<TResponse>> PostAsync<TResponse>(string endpoint, object body)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, body);
        return await ParseResponseAsync<TResponse>(response);
    }
    
    /// <summary>
    /// Generic GET request
    /// </summary>
    public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        return await ParseResponseAsync<TResponse>(response);
    }
    
    /// <summary>
    /// Parse HTTP response into ApiResponse wrapper
    /// </summary>
    private async Task<ApiResponse<T>> ParseResponseAsync<T>(HttpResponseMessage response)
    {
        var statusCode = (int)response.StatusCode;
        
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>();
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Data = data
            };
        }
        
        // Error response
        var errorContent = await response.Content.ReadAsStringAsync();
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Error = errorContent
        };
    }
}
