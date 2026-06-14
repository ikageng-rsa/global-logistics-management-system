using GLMS.Web.Models;
using GLMS.Web.Repositories.Contracts;
using GLMS.Web.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers
{
    [Authorize]

    public class ClientsController : Controller
    {
        private readonly IClientApiService _clientApiService;

        public ClientsController(IClientApiService clientApiService)
        {
            _clientApiService = clientApiService;
        }

        [HttpGet("clients")]
        public async Task<IActionResult> Index()
        {
            var clients = await _clientApiService.GetAllAsync();
            return View(clients);
        }

        [HttpGet("clients/details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientApiService.GetByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpGet("clients/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("clients/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                var success = await _clientApiService.CreateAsync(client);

                if (success)
                {
                    TempData["Success"] = "Client created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to create client. Please try again.");
            }

            return View(client);
        }

        [HttpGet("clients/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientApiService.GetByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost("clients/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var success = await _clientApiService.UpdateAsync(client);

                if (success)
                {
                    TempData["Success"] = "Client updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to update client. Please try again.");
            }

            return View(client);
        }

        [HttpGet("clients/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientApiService.GetByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost("clients/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clientApiService.DeleteAsync(id);
            TempData["Success"] = "Client deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}