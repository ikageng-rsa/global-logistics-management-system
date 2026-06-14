using GLMS.Web.Models;
using GLMS.Web.ViewModels;

namespace GLMS.Web.Services.Contracts
{
    public interface IUserApiService
    {
        Task<IEnumerable<UserListItem>> GetAllAsync();
        Task<IEnumerable<string>> GetRolesAsync();
        Task<(bool Success, string? Error)> CreateAsync(UserCreateViewModel model);
        Task<(bool Success, string? Error)> UpdateAsync(UserEditViewModel model);
        Task<(bool Success, string? Error)> DeleteAsync(string id);
    }
}