using GLMS.Web.Models;

namespace GLMS.Web.Services.Contracts
{
    public interface IServiceRequestApiService
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<(bool Success, string? Error)> CreateAsync(ServiceRequest serviceRequest);
        Task<(bool Success, string? Error)> UpdateAsync(ServiceRequest serviceRequest);
        Task<bool> DeleteAsync(int id);
    }
}