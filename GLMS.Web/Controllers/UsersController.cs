using GLMS.Web.Services.Contracts;
using GLMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserApiService _userApiService;

        public UsersController(IUserApiService userApiService)
        {
            _userApiService = userApiService;
        }


        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var users = await _userApiService.GetAllAsync();
            return View(users);
        }

        [HttpGet("users/create")]
        public async Task<IActionResult> Create()
        {
            await PopulateRoleDropdown();
            return View();
        }


        [HttpPost("users/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (success, error) = await _userApiService.CreateAsync(model);

                if (success)
                {
                    TempData["Success"] = $"User {model.Email} created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, error ?? "Failed to create user.");
            }

            await PopulateRoleDropdown();
            return View(model);
        }

        [HttpGet("users/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var users = await _userApiService.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };

            await PopulateRoleDropdown();
            return View(model);
        }

        [HttpPost("users/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (success, error) = await _userApiService.UpdateAsync(model);

                if (success)
                {
                    TempData["Success"] = $"User {model.Email} updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, error ?? "Failed to update user.");
            }

            await PopulateRoleDropdown();
            return View(model);
        }

        [HttpGet("users/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var users = await _userApiService.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            return View(new UserListViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            });
        }

        [HttpPost("users/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var (success, error) = await _userApiService.DeleteAsync(id);

            if (!success)
            {
                TempData["Error"] = error ?? "Failed to delete user.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateRoleDropdown()
        {
            var roles = await _userApiService.GetRolesAsync();
            ViewBag.Roles = roles.ToList();
        }
    }
}