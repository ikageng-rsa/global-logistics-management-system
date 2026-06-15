using GLMS.Api.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace GLMS.Api.Tests
{
    public class ClientsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ClientsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetClients_WithoutToken_Returns401()
        {
            // Act
            var response = await _client.GetAsync("api/clients");

            // Assert that endpoint is protected, no access without token
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetClients_WithValidToken_Returns200()
        {
            var token = await TestAuthHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("api/clients");

            // Assert that status is okay due to valid token received
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostClient_WithValidToken_Returns201()
        {
            var token = await TestAuthHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var newClient = new { name = "Test Client", contactDetails = "test@test.com", region = "Gauteng" };

            var response = await _client.PostAsJsonAsync("api/clients", newClient);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task DeleteClient_AsUser_Returns403()
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", new
            {
                email = "user@techmore.co.za",
                password = "User@123"
            });

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var token = result?["token"] ?? string.Empty;

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var deleteResponse = await _client.DeleteAsync("api/clients/1");

            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteClient_AsAdmin_Returns404WhenNotFound()
        {
            var token = await TestAuthHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var deleteResponse = await _client.DeleteAsync("api/clients/99999");

            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }
    }
}