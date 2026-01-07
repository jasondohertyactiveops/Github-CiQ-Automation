using Microsoft.Playwright;
using AO.Automation.API.Client.Models;

namespace AO.Automation.API.Client.Helpers;

/// <summary>
/// API client wrapper using Playwright APIRequestContext
/// </summary>
public class ApiHelper : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IAPIRequestContext? _requestContext;
    private readonly string _baseUrl;
    
    public ApiHelper()
    {
        _baseUrl = ApiTestConfig.Instance.ApiBaseUrl;
    }
    
    /// <summary>
    /// Initialize Playwright and create API request context
    /// </summary>
    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _requestContext = await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = _baseUrl,
            IgnoreHTTPSErrors = true
        });
    }
    
    /// <summary>
    /// Set authentication token for subsequent requests
    /// Call this to add Bearer token to all future requests
    /// </summary>
    public async Task SetAuthTokenAsync(string token)
    {
        if (_playwright == null)
            throw new InvalidOperationException("ApiHelper not initialized. Call InitializeAsync first.");
        
        // Dispose old context and create new one with auth header
        if (_requestContext != null)
        {
            await _requestContext.DisposeAsync();
        }
        
        _requestContext = await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = _baseUrl,
            IgnoreHTTPSErrors = true,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {token}"
            }
        });
    }
    
    /// <summary>
    /// Generic POST request
    /// </summary>
    public async Task<ApiResponse<TResponse>> PostAsync<TResponse>(string endpoint, object body)
    {
        if (_requestContext == null)
            throw new InvalidOperationException("ApiHelper not initialized. Call InitializeAsync first.");
        
        var response = await _requestContext.PostAsync(endpoint, new()
        {
            DataObject = body
        });
        
        return await ParseResponseAsync<TResponse>(response);
    }
    
    /// <summary>
    /// Generic GET request
    /// </summary>
    public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string endpoint)
    {
        if (_requestContext == null)
            throw new InvalidOperationException("ApiHelper not initialized. Call InitializeAsync first.");
        
        var response = await _requestContext.GetAsync(endpoint);
        return await ParseResponseAsync<TResponse>(response);
    }
    
    /// <summary>
    /// Generic PUT request
    /// </summary>
    public async Task<ApiResponse<TResponse>> PutAsync<TResponse>(string endpoint, object body)
    {
        if (_requestContext == null)
            throw new InvalidOperationException("ApiHelper not initialized. Call InitializeAsync first.");
        
        var response = await _requestContext.PutAsync(endpoint, new()
        {
            DataObject = body
        });
        
        return await ParseResponseAsync<TResponse>(response);
    }
    
    /// <summary>
    /// Parse Playwright API response into ApiResponse wrapper
    /// </summary>
    private async Task<ApiResponse<T>> ParseResponseAsync<T>(IAPIResponse response)
    {
        var statusCode = response.Status;
        
        if (response.Ok)
        {
            var data = await response.JsonAsync<T>();
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Data = data
            };
        }
        
        // Error response
        var errorContent = await response.TextAsync();
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Error = errorContent
        };
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_requestContext != null)
        {
            await _requestContext.DisposeAsync();
        }
        _playwright?.Dispose();
    }
}
