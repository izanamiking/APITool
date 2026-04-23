using System;
using System.Net.Http;
using System.Threading.Tasks;
using APITool.Models;

namespace APITool.Services
{
    public class ApiService
    {
        public async Task<ApiResponse> SendAsync(ApiRequest request)
        {
            if (request == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Request is null"
                };
            }

            if (string.IsNullOrWhiteSpace(request.Url))
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "URL is empty"
                };
            }

            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out Uri uri))
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid URL"
                };
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    HttpResponseMessage response;

                    if (request.Method == HttpMethodType.GET)
                    {
                        response = await client.GetAsync(uri);
                    }
                    else if (request.Method == HttpMethodType.POST)
                    {
                        var content = new StringContent(
                            request.Body ?? "",
                            System.Text.Encoding.UTF8,
                            "application/json"
                        );

                        response = await client.PostAsync(uri, content);
                    }
                    else
                    {
                        return new ApiResponse
                        {
                            IsSuccess = false,
                            ErrorMessage = "Unsupported method"
                        };
                    }

                    string body = await response.Content.ReadAsStringAsync();

                    return new ApiResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Body = body,
                        IsSuccess = response.IsSuccessStatusCode
                    };
                }
            }
                catch (TaskCanceledException)
            {
                    return new ApiResponse
                {
                        IsSuccess = false,
                        ErrorMessage = "Request timed out"
                };
            }
                catch (Exception ex)
            {
                    return new ApiResponse
                {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                };
            }
        }
    }
}