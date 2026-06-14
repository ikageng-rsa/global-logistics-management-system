using GLMS.Web.Models;

namespace GLMS.Web.Services.Contracts
{
    public interface IClientApiService
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Client client);
        Task<bool> UpdateAsync(Client client);
        Task<bool> DeleteAsync(int id);
    }
}