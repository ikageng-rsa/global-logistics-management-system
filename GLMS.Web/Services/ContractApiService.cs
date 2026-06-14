using System.Net.Http.Json;
using GLMS.Web.Enums;
using GLMS.Web.Models;
using GLMS.Web.Services.Contracts;

namespace GLMS.Web.Services
{
    public class ContractApiService : IContractApiService
    {
        private readonly HttpClient _httpClient;

        public ContractApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync(ContractStatus? status = null)
        {
            var url = "api/contracts";

            if (status.HasValue)
                url += $"?status={status.Value}";

            var contracts = await _httpClient.GetFromJsonAsync<List<Contract>>(url);
            return contracts ?? new List<Contract>();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/contracts/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Contract>();
        }

        public async Task<(bool Success, string? Error, Contract? Contract)> CreateAsync(
            int clientId, DateTime startDate, DateTime endDate, ServiceLevel serviceLevel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/contracts", new
            {
                clientId,
                startDate,
                endDate,
                serviceLevel
            });

            if (response.IsSuccessStatusCode)
            {
                var contract = await response.Content.ReadFromJsonAsync<Contract>();
                return (true, null, contract);
            }

            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var message = error != null && error.ContainsKey("error") ? error["error"] : "Failed to create contract.";

            return (false, message, null);
        }

        public async Task<bool> UpdateStatusAsync(int id, ContractStatus status)
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/contracts/{id}/status", new
            {
                status
            });

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/contracts/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<(bool Success, string? FilePath)> UploadAgreementAsync(int id, IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);

            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "agreementFile", file.FileName);

            var response = await _httpClient.PostAsync($"api/contracts/{id}/agreement", content);

            if (!response.IsSuccessStatusCode)
                return (false, null);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var filePath = result != null && result.ContainsKey("filePath") ? result["filePath"] : null;

            return (true, filePath);
        }
    }
}