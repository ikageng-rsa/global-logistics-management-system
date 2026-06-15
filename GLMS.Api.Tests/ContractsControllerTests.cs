using System.Net;
using System.Net.Http.Json;
using GLMS.Api.Tests.Helpers;

namespace GLMS.Api.Tests
{
    public class ContractsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ContractsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetContracts_WithoutToken_Returns401()
        {
            var response = await _client.GetAsync("api/contracts");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_WithValidToken_Returns200()
        {
            var token = await TestAuthHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("api/contracts");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostContract_InvalidDuration_Returns400()
        {
            // Standard contract with less than 30 days
            var token = await TestAuthHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                clientId = 1,
                startDate = DateTime.Today,
                endDate = DateTime.Today.AddDays(10),
                serviceLevel = "Standard"
            };

            var response = await _client.PostAsJsonAsync("api/contracts", request);

            // Assert that factory validation should reject this
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PatchContractStatus_AsUser_Returns403()
        {
            // login as regular user
            var loginResponse = await _client.PostAsJsonAsync("api/auth/login", new
            {
                email = "user@techmore.co.za",
                password = "User@123"
            });

            var result = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var token = result?["token"] ?? string.Empty;

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PatchAsJsonAsync("api/contracts/1/status", new { status = "Expired" });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}