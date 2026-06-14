using GLMS.Web.Enums;
using GLMS.Web.Models;

namespace GLMS.Web.Services.Contracts
{
    public interface IContractApiService
    {
        Task<IEnumerable<Contract>> GetAllAsync(ContractStatus? status = null);
        Task<Contract?> GetByIdAsync(int id);
        Task<(bool Success, string? Error, Contract? Contract)> CreateAsync(int clientId, DateTime startDate, DateTime endDate, ServiceLevel serviceLevel);
        Task<bool> UpdateStatusAsync(int id, ContractStatus status);
        Task<bool> DeleteAsync(int id);
        Task<(bool Success, string? FilePath)> UploadAgreementAsync(int id, IFormFile file);
    }
}