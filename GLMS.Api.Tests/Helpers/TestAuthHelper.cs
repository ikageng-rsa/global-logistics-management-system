using System.Net.Http.Json;

namespace GLMS.Api.Tests.Helpers
{
    public static class TestAuthHelper
    {
        public static async Task<string> GetAdminTokenAsync(HttpClient client)
        {
            var response = await client.PostAsJsonAsync("api/auth/login", new
            {
                email = "admin@techmore.co.za",
                password = "Admin@123"
            });

            var result = await response.Content
                .ReadFromJsonAsync<Dictionary<string, string>>();

            return result?["token"] ?? string.Empty;
        }
    }
}