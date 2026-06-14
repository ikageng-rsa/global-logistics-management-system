namespace GLMS.Web.Services.Contracts
{
    public interface IAuthApiService
    {
        Task<(bool Success, string? Token, string? Role, string? Error)> LoginAsync(string email, string password);
    }
}