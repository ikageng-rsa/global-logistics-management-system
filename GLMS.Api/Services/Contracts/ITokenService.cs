using Microsoft.AspNetCore.Identity;

namespace GLMS.Api.Services.Contracts
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(IdentityUser user);
    }
}