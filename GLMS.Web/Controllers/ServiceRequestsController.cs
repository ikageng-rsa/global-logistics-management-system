using GLMS.Web.Enums;
using GLMS.Web.Models;
using GLMS.Web.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Web.Controllers
{
    [Authorize]

    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestApiService _serviceRequestApiService;
        private readonly IContractApiService _contractApiService;

        public ServiceRequestsController(
            IServiceRequestApiService serviceRequestApiService,
            IContractApiService contractApiService)
        {
            _serviceRequestApiService = serviceRequestApiService;
            _contractApiService = contractApiService;
        }

        [HttpGet("service-requests")]
        public async Task<IActionResult> Index()
        {
            var requests = await _serviceRequestApiService.GetAllAsync();
            return View(requests);
        }

        [HttpGet("service-requests/details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var request = await _serviceRequestApiService.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpGet("service-requests/create")]
        public async Task<IActionResult> Create()
        {
            await PopulateContractDropdown();
            return View();
        }

        [HttpPost("service-requests/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            if (!ModelState.IsValid)
            {
                await PopulateContractDropdown();
                return View(serviceRequest);
            }

            var (success, error) = await _serviceRequestApiService.CreateAsync(serviceRequest);

            if (success)
            {
                TempData["Success"] = "Service request created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error ?? "Failed to create service request.");
            await PopulateContractDropdown();
            return View(serviceRequest);
        }

        [HttpGet("service-requests/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var request = await _serviceRequestApiService.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost("service-requests/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(serviceRequest);

            var (success, error) = await _serviceRequestApiService.UpdateAsync(serviceRequest);

            if (success)
            {
                TempData["Success"] = "Service request updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error ?? "Failed to update service request.");
            return View(serviceRequest);
        }

        [HttpGet("service-requests/delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _serviceRequestApiService.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost("service-requests/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceRequestApiService.DeleteAsync(id);
            TempData["Success"] = "Service request deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateContractDropdown()
        {
            // Only show Active contracts in the dropdown
            var activeContracts = (await _contractApiService.GetAllAsync(ContractStatus.Active))
                .Select(c => new
                {
                    c.Id,
                    Label = $"{c.Client?.Name} — {c.ServiceLevel} ({c.StartDate:dd MMM yyyy} to {c.EndDate:dd MMM yyyy})"
                })
                .ToList();

            ViewBag.Contracts = new SelectList(activeContracts, "Id", "Label");
        }
    }
}