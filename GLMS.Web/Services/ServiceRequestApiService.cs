using System.Net.Http.Json;
using GLMS.Web.Models;
using GLMS.Web.Services.Contracts;

namespace GLMS.Web.Services
{
    public class ServiceRequestApiService : IServiceRequestApiService
    {
        private readonly HttpClient _httpClient;

        public ServiceRequestApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
        {
            var requests = await _httpClient.GetFromJsonAsync<List<ServiceRequest>>("api/servicerequests");
            return requests ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/servicerequests/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ServiceRequest>();
        }

        public async Task<(bool Success, string? Error)> CreateAsync(ServiceRequest serviceRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/servicerequests", serviceRequest);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var message = error != null && error.ContainsKey("error") ? error["error"] : "Failed to create service request.";

            return (false, message);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(ServiceRequest serviceRequest)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/servicerequests/{serviceRequest.Id}", serviceRequest);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var message = error != null && error.ContainsKey("error") ? error["error"] : "Failed to update service request.";

            return (false, message);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/servicerequests/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}