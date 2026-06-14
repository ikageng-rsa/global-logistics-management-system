using System.Net.Http.Json;
using GLMS.Web.Models;
using GLMS.Web.Services.Contracts;
using GLMS.Web.ViewModels;

namespace GLMS.Web.Services
{
    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<UserListItem>> GetAllAsync()
        {
            var users = await _httpClient.GetFromJsonAsync<List<UserListItem>>("api/users");
            return users ?? new List<UserListItem>();
        }

        public async Task<IEnumerable<string>> GetRolesAsync()
        {
            var roles = await _httpClient.GetFromJsonAsync<List<string>>("api/users/roles");
            return roles ?? new List<string>();
        }

        public async Task<(bool Success, string? Error)> CreateAsync(UserCreateViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users", new
            {
                email = model.Email,
                password = model.Password,
                role = model.Role
            });

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var message = error != null && error.ContainsKey("error") ? error["error"] : "Failed to create user.";

            return (false, message);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(UserEditViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/users/{model.Id}", new
            {
                id = model.Id,
                email = model.Email,
                role = model.Role,
                newPassword = model.NewPassword
            });

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var message = error != null && error.ContainsKey("error") ? error["error"] : "Failed to update user.";

            return (false, message);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/users/{id}");

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var message = error != null && error.ContainsKey("error") ? error["error"] : "Failed to delete user.";

            return (false, message);
        }
    }
}