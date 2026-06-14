using System.Net.Http.Json;
using GLMS.Web.Models;
using GLMS.Web.Services.Contracts;

namespace GLMS.Web.Services
{
    public class ClientApiService : IClientApiService
    {
        private readonly HttpClient _httpClient;

        public ClientApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            var clients = await _httpClient.GetFromJsonAsync<List<Client>>("api/clients");
            return clients ?? new List<Client>();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/clients/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Client>();
        }

        public async Task<bool> CreateAsync(Client client)
        {
            var response = await _httpClient.PostAsJsonAsync("api/clients", client);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Client client)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/clients/{client.Id}", client);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}