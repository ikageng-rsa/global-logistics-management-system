using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GLMS.Api.DTOs;
using GLMS.Api.Services.Contracts;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Unauthorized(new { error = "Invalid email or password." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new { error = "Invalid email or password." });

            var token = await _tokenService.GenerateTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                token,
                email = user.Email,
                role = roles.FirstOrDefault() ?? "User"
            });
        }
    }
}