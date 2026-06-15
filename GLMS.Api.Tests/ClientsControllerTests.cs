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
    }
}