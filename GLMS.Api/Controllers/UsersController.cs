using GLMS.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserListResponse>>> GetAll()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserListResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserListResponse
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            return Ok(result);
        }

        [HttpGet("roles")]
        public ActionResult<IEnumerable<string>> GetRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return Ok(roles);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(new { error = string.Join("; ", result.Errors.Select(e => e.Description)) });

            await _userManager.AddToRoleAsync(user, request.Role);

            return Ok(new UserListResponse { Id = user.Id, Email = user.Email!, Role = request.Role });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID in route does not match ID in body.");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Update email
            user.Email = request.Email;
            user.UserName = request.Email;
            await _userManager.UpdateAsync(user);

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Role);

            // Update password if provided
            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // Prevent admin from deleting their own account
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (currentUserId == id)
                return BadRequest(new { error = "You cannot delete your own account." });

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userManager.DeleteAsync(user);
            return NoContent();
        }
    }
}