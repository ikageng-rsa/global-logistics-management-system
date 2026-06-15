using System.Net;
using GLMS.Api.Tests.Helpers;

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
    }
}