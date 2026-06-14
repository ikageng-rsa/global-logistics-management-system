using System.Net.Http.Json;
using System.Text.Json;
using GLMS.Web.Models;
using GLMS.Web.Services.Contracts;

namespace GLMS.Web.Services
{
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;

        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool Success, string? Token, string? Role, string? Error)> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                email,
                password
            });

            if (!response.IsSuccessStatusCode)
                return (false, null, null, "Invalid email or password.");

            var result = await response.Content.ReadFromJsonAsync<LoginResult>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                return (false, null, null, "Unexpected response from server.");

            return (true, result.Token, result.Role, null);
        }
    }
}