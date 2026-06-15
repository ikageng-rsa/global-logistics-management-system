using System.Net;
using System.Net.Http.Json;
using GLMS.Api.Tests.Helpers;

namespace GLMS.Api.Tests
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", new
            {
                email = "admin@techmore.co.za",
                password = "Admin@123"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("token"));
            Assert.NotEmpty(result["token"]);
        }

        [Fact]
        public async Task Login_InvalidCredentials_Returns401()
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", new
            {
                email = "wrong@email.com",
                password = "WrongPassword"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsCorrectRole()
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", new
            {
                email = "admin@techmore.co.za",
                password = "Admin@123"
            });

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            Assert.NotNull(result);
            Assert.Equal("Admin", result["role"]);
        }
    }
}