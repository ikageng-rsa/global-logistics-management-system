using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GLMS.Web.ViewModels;

namespace GLMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
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

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserListViewModel
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            return View(userList);
        }

        [HttpGet("users/create")]
        public IActionResult Create()
        {
            PopulateRoleDropdown();
            return View();
        }

        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["Success"] = $"User {model.Email} created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            PopulateRoleDropdown();
            return View(model);
        }

        [HttpGet("users/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? string.Empty
            };

            PopulateRoleDropdown();
            return View(model);
        }

        [HttpPost("users/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                // Update email
                user.Email = model.Email;
                user.UserName = model.Email;
                await _userManager.UpdateAsync(user);

                // Update role
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);

                // Update password if provided
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                }

                TempData["Success"] = $"User {model.Email} updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            PopulateRoleDropdown();
            return View(model);
        }

        [HttpGet("users/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserListViewModel
            {
                Id = user.Id,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "No Role"
            };

            return View(model);
        }

        [HttpPost("users/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Prevent admin from deleting their own account
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == id)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.DeleteAsync(user);

            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateRoleDropdown()
        {
            ViewBag.Roles = _roleManager.Roles
                .Select(r => r.Name)
                .ToList();
        }
    }
}